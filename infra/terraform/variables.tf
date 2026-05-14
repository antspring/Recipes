variable "cloud_id" {
  description = "Yandex Cloud ID."
  type        = string
}

variable "folder_id" {
  description = "Yandex Cloud folder ID."
  type        = string
}

variable "zone" {
  description = "Default Yandex Cloud zone."
  type        = string
  default     = "ru-central1-a"
}

variable "service_account_key_file" {
  description = "Path to the Yandex Cloud service account key JSON file."
  type        = string
}

variable "project_name" {
  description = "Short project name used for Yandex Cloud resource names."
  type        = string
  default     = "recipes"
}

variable "subnet_cidr" {
  description = "CIDR block for the application subnet."
  type        = list(string)
  default     = ["10.10.0.0/24"]
}

variable "ssh_user" {
  description = "Linux user for SSH access."
  type        = string
  default     = "ubuntu"
}

variable "ssh_public_key" {
  description = "Optional SSH public key added to VM metadata."
  type        = string
  default     = ""
  sensitive   = true
}

variable "instance_count" {
  description = "Number of VM instances in the fixed-size instance group."
  type        = number
  default     = 2
}

variable "platform_id" {
  description = "Yandex Compute platform ID."
  type        = string
  default     = "standard-v3"
}

variable "cores" {
  description = "CPU cores per VM."
  type        = number
  default     = 2
}

variable "core_fraction" {
  description = "CPU core fraction per VM."
  type        = number
  default     = 20
}

variable "memory" {
  description = "Memory in GB per VM."
  type        = number
  default     = 2
}

variable "boot_disk_size" {
  description = "Boot disk size in GB."
  type        = number
  default     = 20
}

variable "boot_disk_type" {
  description = "Boot disk type."
  type        = string
  default     = "network-hdd"
}

variable "api_port" {
  description = "Port exposed by the API container on each VM."
  type        = number
  default     = 8080
}

variable "lb_listener_port" {
  description = "Public Network Load Balancer listener port."
  type        = number
  default     = 80
}

variable "allowed_ssh_cidrs" {
  description = "Comma-separated CIDR blocks allowed to connect to VM SSH. Leave empty to disable SSH ingress."
  type        = string
  default     = ""
}

variable "dockerhub_username" {
  description = "Docker Hub username used for docker login on VMs."
  type        = string
  sensitive   = true
}

variable "dockerhub_token" {
  description = "Docker Hub token used for docker login on VMs."
  type        = string
  sensitive   = true
}

variable "dockerhub_namespace" {
  description = "Docker Hub namespace containing application images. Defaults to dockerhub_username."
  type        = string
  default     = ""
}

variable "api_image_name" {
  description = "API image repository name."
  type        = string
  default     = "recipes-api"
}

variable "migrations_image_name" {
  description = "Migrations image repository name."
  type        = string
  default     = "recipes-migrations"
}

variable "image_tag" {
  description = "Docker image tag to deploy."
  type        = string
  default     = "latest"
}

variable "deployment_version" {
  description = "Opaque deployment version. Change it to force an instance group rolling update while keeping image_tag stable."
  type        = string
  default     = ""
}

variable "aspnetcore_environment" {
  description = "ASPNETCORE_ENVIRONMENT value for the API container."
  type        = string
  default     = "Production"
}

variable "postgresql_connection_string" {
  description = "PostgreSQL connection string for API and migrations."
  type        = string
  sensitive   = true
}

variable "jwt_issuer" {
  description = "JWT issuer."
  type        = string
}

variable "jwt_audience" {
  description = "JWT audience."
  type        = string
}

variable "jwt_key" {
  description = "JWT signing key."
  type        = string
  sensitive   = true
}

variable "jwt_access_expiration_minutes" {
  description = "JWT access token lifetime in minutes."
  type        = number
  default     = 60
}

variable "jwt_refresh_expiration_days" {
  description = "JWT refresh token lifetime in days."
  type        = number
  default     = 30
}

variable "object_storage_endpoint" {
  description = "S3-compatible object storage endpoint."
  type        = string
  default     = "https://storage.yandexcloud.net"
}

variable "object_storage_bucket" {
  description = "Object storage bucket name."
  type        = string
}

variable "object_storage_access_key" {
  description = "Object storage access key."
  type        = string
  sensitive   = true
}

variable "object_storage_secret_key" {
  description = "Object storage secret key."
  type        = string
  sensitive   = true
}

variable "object_storage_region" {
  description = "Object storage region."
  type        = string
  default     = "ru-central1"
}

variable "smtp_host" {
  description = "SMTP host."
  type        = string
}

variable "smtp_port" {
  description = "SMTP port."
  type        = number
  default     = 587
}

variable "smtp_username" {
  description = "SMTP username."
  type        = string
  default     = ""
  sensitive   = true
}

variable "smtp_password" {
  description = "SMTP password."
  type        = string
  default     = ""
  sensitive   = true
}

variable "smtp_from_email" {
  description = "Email address used as sender."
  type        = string
}

variable "smtp_from_name" {
  description = "Display name used as sender."
  type        = string
  default     = "Recipes"
}

variable "smtp_use_ssl" {
  description = "Whether SMTP should use SSL."
  type        = bool
  default     = true
}
