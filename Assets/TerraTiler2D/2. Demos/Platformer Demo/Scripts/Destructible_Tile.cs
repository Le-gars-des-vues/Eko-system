using UnityEngine;
using UnityEngine.UI;

namespace TerraTiler2D
{
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Destructible_Tile : MonoBehaviour
    {
        private ParticleSystem myParticleSystem;

        public int hitsRequired = 1;

        private void Start()
        {
            myParticleSystem = GetComponent<ParticleSystem>();
        }

        public void DamageTile()
        {
            if (hitsRequired > 0)
            {
                hitsRequired--;
                myParticleSystem.Play();

                if (hitsRequired <= 0)
                {
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<SpriteRenderer>().enabled = false;
                    this.enabled = false;
                }
            }
        }
    }
}
