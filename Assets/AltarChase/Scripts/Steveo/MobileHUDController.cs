using Mirror;
using UnityEngine;
public class MobileHUDController : NetworkBehaviour
{
    [SerializeField] private GameObject mobileControls;
    private void Start() 
    {
    #if UNITY_ANDROID
        mobileControls.SetActive(true);
    #else
        mobileControls.SetActive(false);
    #endif
    }
    private void Update() 
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
        {
            mobileControls.SetActive(!mobileControls.activeSelf);
        }
    }
}

