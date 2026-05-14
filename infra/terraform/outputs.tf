output "network_load_balancer_id" {
  description = "Yandex Network Load Balancer ID."
  value       = yandex_lb_network_load_balancer.app.id
}

output "network_load_balancer_address" {
  description = "Public IPv4 address of the Network Load Balancer."
  value       = try(tolist(yandex_lb_network_load_balancer.app.listener)[0].external_address_spec[0].address, null)
}

output "instance_group_id" {
  description = "Compute instance group ID."
  value       = yandex_compute_instance_group.app.id
}

output "target_group_id" {
  description = "Network Load Balancer target group ID created by the instance group."
  value       = yandex_compute_instance_group.app.load_balancer[0].target_group_id
}

output "api_image" {
  description = "API image deployed by cloud-init."
  value       = nonsensitive(local.api_image)
}

output "migrations_image" {
  description = "Migrations image deployed by cloud-init."
  value       = nonsensitive(local.migrations_image)
}
