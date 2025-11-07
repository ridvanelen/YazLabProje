using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Ateş Etme Ayarları")]
    public Camera playerCam; 
    public float damage = 25f;
    public float range = 100f;

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    
    void Shoot()
    {
        RaycastHit hit;
        

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {

            Debug.Log("Çarptı: " + hit.transform.name);

            EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
            
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("DÜŞMANA HASAR VERİLDİ!");
            }
        }
    }
}