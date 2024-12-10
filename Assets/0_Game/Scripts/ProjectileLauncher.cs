using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
	[System.Serializable]
	public class ProjectileData
	{
		public string name; // Tên loại đạn (mũi tên, lưỡi kiếm, ...)
		public GameObject prefab; // Prefab của đạn
		public float lifetime = 5f; // Thời gian tồn tại
	}

	public List<ProjectileData> projectiles; // Danh sách các loại đạn
	public Transform spawnPoint;

	public void FireProjectile(string projectileName)
	{
		// Tìm loại đạn dựa trên tên
		ProjectileData selectedProjectile = projectiles.Find(p => p.name == projectileName);

		if (selectedProjectile != null)
		{
			GameObject projectile = Instantiate(selectedProjectile.prefab, spawnPoint.position, selectedProjectile.prefab.transform.rotation);

			// Định hướng đạn theo hướng nhân vật
			Vector3 oriScale = projectile.transform.localScale;
			projectile.transform.localScale = new Vector3(
				oriScale.x * Mathf.Sign(transform.localScale.x),
				oriScale.y,
				oriScale.z
			);

			Destroy(projectile, selectedProjectile.lifetime);
		}
		else
		{
			Debug.LogWarning("Projectile with name " + projectileName + " not found!");
		}
	}
}
