locals {
  docker_namespace = var.dockerhub_namespace != "" ? var.dockerhub_namespace : var.dockerhub_username
  api_image        = "${local.docker_namespace}/${var.api_image_name}:${var.image_tag}"
  migrations_image = "${local.docker_namespace}/${var.migrations_image_name}:${var.image_tag}"

  labels = {
    project = var.project_name
  }

  allowed_ssh_cidrs = compact([
    for cidr in split(",", var.allowed_ssh_cidrs) : trimspace(cidr)
  ])

  app_environment = {
    ASPNETCORE_ENVIRONMENT                        = var.aspnetcore_environment
    ConnectionStrings__PostgreSqlConnectionString = var.postgresql_connection_string
    Jwt__Issuer                                   = var.jwt_issuer
    Jwt__Audience                                 = var.jwt_audience
    Jwt__Key                                      = var.jwt_key
    Jwt__AccessExpirationMinutes                  = tostring(var.jwt_access_expiration_minutes)
    Jwt__RefreshExpirationDays                    = tostring(var.jwt_refresh_expiration_days)
    ObjectStorage__Endpoint                       = var.object_storage_endpoint
    ObjectStorage__Bucket                         = var.object_storage_bucket
    ObjectStorage__AccessKey                      = var.object_storage_access_key
    ObjectStorage__SecretKey                      = var.object_storage_secret_key
    ObjectStorage__Region                         = var.object_storage_region
    Smtp__Host                                    = var.smtp_host
    Smtp__Port                                    = tostring(var.smtp_port)
    Smtp__UserName                                = var.smtp_username
    Smtp__Password                                = var.smtp_password
    Smtp__FromEmail                               = var.smtp_from_email
    Smtp__FromName                                = var.smtp_from_name
    Smtp__UseSsl                                  = tostring(var.smtp_use_ssl)
  }

  app_env_content = join("\n", [
    for key, value in local.app_environment : "${key}=${value}"
  ])

  ssh_metadata = var.ssh_public_key == "" ? {} : {
    ssh-keys = "${var.ssh_user}:${var.ssh_public_key}"
  }
}

data "yandex_compute_image" "ubuntu" {
  family    = "ubuntu-2404-lts"
  folder_id = "standard-images"
}

resource "yandex_vpc_network" "app" {
  name   = "${var.project_name}-network"
  labels = local.labels
}

resource "yandex_vpc_subnet" "app" {
  name           = "${var.project_name}-subnet"
  zone           = var.zone
  network_id     = yandex_vpc_network.app.id
  v4_cidr_blocks = var.subnet_cidr
  labels         = local.labels
}

resource "yandex_vpc_security_group" "app" {
  name       = "${var.project_name}-app-sg"
  network_id = yandex_vpc_network.app.id
  labels     = local.labels

  ingress {
    description    = "API traffic from Network Load Balancer"
    protocol       = "TCP"
    port           = var.api_port
    v4_cidr_blocks = ["0.0.0.0/0"]
  }

  dynamic "ingress" {
    for_each = length(local.allowed_ssh_cidrs) == 0 ? [] : [1]

    content {
      description    = "SSH access"
      protocol       = "TCP"
      port           = 22
      v4_cidr_blocks = local.allowed_ssh_cidrs
    }
  }

  egress {
    description    = "Outbound access"
    protocol       = "ANY"
    v4_cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "yandex_iam_service_account" "instance_group" {
  name        = "${var.project_name}-ig-sa"
  description = "Service account for ${var.project_name} instance group."
}

resource "yandex_resourcemanager_folder_iam_member" "instance_group_compute_editor" {
  folder_id = var.folder_id
  role      = "compute.editor"
  member    = "serviceAccount:${yandex_iam_service_account.instance_group.id}"
}

resource "yandex_resourcemanager_folder_iam_member" "instance_group_lb_editor" {
  folder_id = var.folder_id
  role      = "load-balancer.editor"
  member    = "serviceAccount:${yandex_iam_service_account.instance_group.id}"
}

resource "yandex_resourcemanager_folder_iam_member" "instance_group_vpc_public_admin" {
  folder_id = var.folder_id
  role      = "vpc.publicAdmin"
  member    = "serviceAccount:${yandex_iam_service_account.instance_group.id}"
}

resource "yandex_compute_instance_group" "app" {
  name                = "${var.project_name}-ig"
  folder_id           = var.folder_id
  service_account_id  = yandex_iam_service_account.instance_group.id
  deletion_protection = false
  labels              = local.labels

  depends_on = [
    yandex_resourcemanager_folder_iam_member.instance_group_compute_editor,
    yandex_resourcemanager_folder_iam_member.instance_group_lb_editor,
    yandex_resourcemanager_folder_iam_member.instance_group_vpc_public_admin
  ]

  instance_template {
    platform_id = var.platform_id

    resources {
      cores         = var.cores
      core_fraction = var.core_fraction
      memory        = var.memory
    }

    boot_disk {
      mode = "READ_WRITE"

      initialize_params {
        image_id = data.yandex_compute_image.ubuntu.id
        size     = var.boot_disk_size
        type     = var.boot_disk_type
      }
    }

    network_interface {
      network_id         = yandex_vpc_network.app.id
      subnet_ids         = [yandex_vpc_subnet.app.id]
      nat                = true
      security_group_ids = [yandex_vpc_security_group.app.id]
    }

    metadata = merge(local.ssh_metadata, {
      deployment-version = var.deployment_version
      user-data = templatefile("${path.module}/templates/cloud-init.yaml.tftpl", {
        api_image           = local.api_image
        app_env_content     = local.app_env_content
        api_port            = var.api_port
        docker_username_b64 = base64encode(var.dockerhub_username)
        docker_token_b64    = base64encode(var.dockerhub_token)
      })
    })
  }

  scale_policy {
    fixed_scale {
      size = var.instance_count
    }
  }

  allocation_policy {
    zones = [var.zone]
  }

  deploy_policy {
    max_unavailable = 1
    max_expansion   = 1
  }

  load_balancer {
    target_group_name        = "${var.project_name}-target-group"
    target_group_description = "Network Load Balancer target group for ${var.project_name}."
  }
}

resource "yandex_lb_network_load_balancer" "app" {
  name   = "${var.project_name}-nlb"
  labels = local.labels

  listener {
    name        = "http"
    port        = var.lb_listener_port
    target_port = var.api_port

    external_address_spec {
      ip_version = "ipv4"
    }
  }

  attached_target_group {
    target_group_id = yandex_compute_instance_group.app.load_balancer[0].target_group_id

    healthcheck {
      name = "tcp-${var.api_port}"

      tcp_options {
        port = var.api_port
      }
    }
  }
}
