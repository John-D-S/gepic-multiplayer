using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltarChase.Player
{
    /// <summary>
    /// This class handles all the player interactions with environment.
    /// </summary>
    public class PlayerInteract : MonoBehaviour
    {
        /* pick up traps
         * trap number & update HUD
         * set traps
         * disable motor when hitting trap
         * pick up artifact
         * is holding artifact
         * drop artifact
         * WIN
         * LOOSE ??
         * MOBILE INPUT - onscreen buttons for trap setting.
         */

        [SerializeField] private GameObject trap;

        [SerializeField] private int trapCount = 0;


        /// <summary>
        /// Function for dropping traps.
        /// </summary>
        private void DropTrap()
        {
	        // Calculate the distance to the ground from the player character.
	        float dist = 0;
	        Ray ray = new Ray(transform.position, Vector3.down);
	        RaycastHit hit;
	        if(Physics.Raycast(ray, out hit, 5f))
	        {
		        dist = hit.distance;
	        }

	        // todo will have to be moved into the networked player script.
	        
	        if(trapCount > 0)
	        {
		        // Use the calculated distance to set the position for the trap.
		        Vector3 position = new Vector3(transform.position.x, transform.position.y - dist, transform.position.z);
		        Instantiate(trap, position, Quaternion.identity);
		        // Minus 1 from the trap count.
		        trapCount -= 1;
	        }
	        else
	        {
		        Debug.Log("No Traps left.");
		        // todo UI feedback, no traps.
	        }
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
	        if(Input.GetKeyDown(KeyCode.Space))
	        {
		        DropTrap();
	        }
        }
    }
}