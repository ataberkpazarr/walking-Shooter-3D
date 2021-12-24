using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using System.Threading;
using UnityEngine.UI;


public class thirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera; // reference to virtual camera which follows the shooting behaviour
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    private Transform bulletProjectilePrefab;

    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform holdGunPosition;
    [SerializeField] private GameObject gunToBeHold;

    [SerializeField] private Transform redBulletProjectilePrefab;
    [SerializeField] private Transform greenBulletProjectilePrefab;
    [SerializeField] private Transform yellowBulletProjectilePrefab;

    [SerializeField] private Text scoreBoard;


    [SerializeField] private float maxAngleForDeviation;


    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private GameObject firedProjectile;

    private GameObject _gun;

    private Quaternion lastQuaternion;
    private Vector3 firstQuaternion;


    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        firedProjectile = null;

    }


    private void Start()
    {
        //make cursor position constant
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _gun = Instantiate(gunToBeHold, holdGunPosition.position, Quaternion.identity) as GameObject;
        lastQuaternion = _gun.transform.rotation;
        firstQuaternion = _gun.transform.position;

        //Child object of gun is for reaching position where the fired projectiles will be spawned
        GameObject ChildGameObject1 = _gun.transform.GetChild(0).gameObject;
        spawnBulletPosition = ChildGameObject1.transform;

        
        // at the start scene, user pressed to buttons to set projectile's colour and button onclicks functions saved its decision accordingly

        if (PlayerPrefs.HasKey("Color"))
        {
            if (PlayerPrefs.GetInt("Color") == 0) //green
            {
                bulletProjectilePrefab = greenBulletProjectilePrefab;
            }
            else if (PlayerPrefs.GetInt("Color") == 1) //red
            {
                bulletProjectilePrefab = redBulletProjectilePrefab;

            }
            else if (PlayerPrefs.GetInt("Color") == 2) // yellow
            {
                bulletProjectilePrefab = yellowBulletProjectilePrefab;

            }
        } 

    }

    private void Update() // new input system
    {
        
        updateScoreboard();

        //below lines provides character to keep the gun and update its position if needed

        _gun.transform.position = holdGunPosition.position; 
        Quaternion currentRotation = transform.rotation;
        Quaternion delta = Quaternion.Inverse(lastQuaternion) * currentRotation;

        // and updating lastRotation for the next check

        lastQuaternion = currentRotation;
        _gun.transform.rotation = lastQuaternion;


        // aim (crosshair) logo position set ups

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }
        
        //aim and shooting operations

        if (starterAssetsInputs.aim) // if mouse right clicked 
        {


            //required game configs for aimed position of character

            aimVirtualCamera.gameObject.SetActive(true); // zoom by enabling related camera which is for aiming issues
            thirdPersonController.setSensitivity(aimSensitivity); // when user right clicks for aiming and the camera is being zoomed, then cameras movement sensitivity is being decreased in order to slow down aim operation in zoomed situation in compare to non-zoomed situation
            thirdPersonController.setRotateOnMove(false); // if zoomed, then character should rotate with respected to zoomed sensitivity


            //rotating gun and character towards the target

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 200f);
            _gun.transform.rotation.SetFromToRotation(_gun.transform.position, transform.forward);


            //go to the related animation state for aiming

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 200f));


            if (starterAssetsInputs.shoot) // if left clicked while already right clicked

            {
                doShoot(mouseWorldPosition);
        
                starterAssetsInputs.shoot = false;
            }

        }

        else  // if aim operation released 
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.setSensitivity(normalSensitivity);
            thirdPersonController.setRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 200f));
       
        }

    }

    private void doShoot(Vector3 vec)
    {

        Vector3 aimDirectionToBe = (vec - spawnBulletPosition.position).normalized;

        //below rand_ value decides the amount of deviation at the every shooting of player
        //the maximum possible deviation angle is being decided by maxAngleForDeviation value which is set by inspector, 10 currently
        //the targeted position by mouse is being multiplied with the rand_ value

        float rand_ = Random.Range(0, maxAngleForDeviation);
        aimDirectionToBe = Quaternion.AngleAxis(rand_, Vector3.up) * aimDirectionToBe;

        //instantiate the projectile
        Instantiate(bulletProjectilePrefab, spawnBulletPosition.position, Quaternion.LookRotation(aimDirectionToBe, Vector3.up));

    }


    private void updateScoreboard()
    {
         GameObject [] activeProjectiles = GameObject.FindGameObjectsWithTag("projectile");
        if (activeProjectiles.Length >0)
        {
            scoreBoard.text = activeProjectiles.Length.ToString();
        }
        else if (activeProjectiles.Length == 0)
        {
            scoreBoard.text = "";
            scoreBoard.text = "00";
        }
    }

}
 