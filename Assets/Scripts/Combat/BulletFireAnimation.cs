using Mirror;
using UnityEngine;

public class BulletFireAnimation : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float destroyAfterSeconds = 5f;

    void Start()
    {
        rb.velocity = transform.forward * launchForce; // launch the bullet
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds); // destroy bullet after 5 seconds
    }

    private void OnTriggerEnter(Collider other) // this built in method gets called whenever the bullet hits something
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if(networkIdentity.connectionToClient == connectionToClient) { return; } // if the bullet hit something on its "team", do nothing
        } 

        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }

        DestroySelf();
    }

    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject); // destroy bullet
    }
}
