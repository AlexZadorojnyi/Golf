using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    //Steam VR
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;
    //Game objects
    public GameObject golfClub;
    public GameObject golfClubHead;
    public GameObject golfClubShaft;
    public GameObject golfBall;
    public GameObject playerArea;
    public GameObject playerHead;
    public GameObject indicator;
    //Components
    LineRenderer laser;
    //Variables
    private bool shift;
    private Vector3 shiftStart;
    private Vector3 shiftEnd;
    private float shiftStartTime;
    public float shiftDuration;
    private bool ballFollowLaser;
    private string mode;

    void Start()
    {
        //TO DO: Parent the golf club to the controller for the initial mode switch
        golfClub.SetActive(false);
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        laser = gameObject.GetComponent<LineRenderer>();
        laser.material.color = Color.red;
        laser.enabled = false;
        indicator.SetActive(false);
        shift = false;
        ballFollowLaser = false;
        mode = "teleport";
    }

    void Update()
    {
        device = SteamVR_Controller.Input((int)trackedObject.index);

        //Change modes
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            ChangeMode();
        }

        //Teleport mode actions
        if (mode == "teleport")
        {
            //Project ray
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit))
            {
                //Show laser
                laser.SetPosition(0, ray.origin);
                laser.SetPosition(1, ray.GetPoint(hit.distance));
                laser.enabled = true;
                //If hit ground or putt area and ball not following
                if (hit.collider.tag == "Ground" || (hit.collider.tag == "Putt Area" && !ballFollowLaser))
                {
                    //Show indicator
                    indicator.SetActive(true);
                    indicator.transform.position = hit.point;
                    //If press
                    if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        //Teleport
                        Shift(hit.point);
                    }
                }
                //If hit golf ball
                /**if (hit.collider.tag == "Golf Ball")
                {
                    //Hide indicator
                    indicator.SetActive(false);
                    //If press
                    if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        //Ball follow
                        BallFollowLaser(true);
                    }
                }**/
                //If hit putt area and ball follow
                /**if (hit.collider.tag == "Putt Area" && ballFollowLaser)
                {
                    Debug.Log("Putt Area && ballFollowLaser");
                    //Ball follow
                    BallFollowLaser(true);
                    golfBall.transform.position = hit.point + new Vector3(0.0F, 0.05F, 0.0F);
                    //If press
                    if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        //Ball unfollow
                        Debug.Log("Unfollow");
                        BallFollowLaser(false);
                        //Show indicator
                        indicator.SetActive(true);
                        indicator.transform.position = hit.point;
                    }
                }**/
                //If hit ground and ball follow
                /**if (hit.collider.tag == "Ground" && ballFollowLaser)
                {
                    Debug.Log("Ground && ballFollowLaser");
                    //Ball unfollow
                    Debug.Log("Unfollow");
                    BallFollowLaser(false);
                    //Show indicator
                    indicator.SetActive(true);
                    indicator.transform.position = hit.point;
                    //If press
                    if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        //Teleport
                        Shift(hit.point);
                    }
                }**/
                if (hit.collider.tag != "Ground" && hit.collider.tag != "Putt Area")
                {
                    indicator.SetActive(false);
                }
            }
            //If nothing hit
            else
            {
                laser.enabled = false;
                indicator.SetActive(false);
            }
            
        }

        //Putt mode actions
        if (mode == "putt")
        {
            //Project ray
            Ray ray = new Ray(transform.position, transform.forward);
            //If press
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) 
            {
                //If angle over 45
                if (Vector3.Angle((ray.GetPoint(1) - ray.origin), Vector3.down) <= 45)
                {
                    DestroyGolfClub();
                    //Debug.Log("Spawn");
                    //Spawn golf club
                    SpawnGolfClub();
                }
                else
                {
                    
                    DestroyGolfClub();
                    //Spawn golf club
                    golfClub.SetActive(true);
                }
            }
        }

        //Shift
        if (shift)
        {
            float distCovered = (Time.time - shiftStartTime) / shiftDuration;
            playerArea.transform.position = Vector3.Lerp(shiftStart, shiftEnd, distCovered);
            if (playerArea.transform.position == shiftEnd) shift = false;
        }

        //Ball
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            golfBall.GetComponent<GolfBall>().Reset();
        }
    }

    //Enables shift mode
    private void Shift(Vector3 dest)
    {
        shift = true;
        shiftStart = playerArea.transform.position;
        shiftEnd = CalcNewPos(dest);
        shiftStartTime = Time.time;
    }

    //Calculates camera rig position to match player position to shift destination
    private Vector3 CalcNewPos(Vector3 point)
    {
        Vector3 diff = new Vector3(playerHead.transform.position.x, 0.0F, playerHead.transform.position.z) - playerArea.transform.position;
        return new Vector3(point.x - diff.x, point.y, point.z - diff.z);
    }

    //Spawns and scales club based on distance of controller from the ground
    private void SpawnGolfClub()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            //Get ground position
            Vector3 ground = new Vector3();
            ground = ray.GetPoint(hit.distance);
            //Get wrist position
            Vector3 hand = new Vector3();
            hand = transform.position;
            //Calculate golf club length
            float golfClubLength = Vector3.Distance(hand, ground);
            //Calculate golf club midpoint position
            Vector3 golfClubMidpoint = new Vector3();
            golfClubMidpoint = (hand + ground) / 2;
            //Activate and position golf club
            golfClub.SetActive(true);
            golfClub.transform.position = golfClubMidpoint;
            golfClub.transform.localScale = new Vector3(golfClub.transform.localScale.x, golfClub.transform.localScale.y, golfClubLength - 0.1F);
            golfClub.transform.rotation = transform.rotation;
            golfClub.transform.SetParent(transform);
        }
    }

    private void DestroyGolfClub()
    {
        golfClub.SetActive(false);
    }

    public void Vibrate()
    {
        SteamVR_Controller.Input((int)trackedObject.index).TriggerHapticPulse(2000);
    }

    private void ChangeMode()
    {
        if (mode == "putt")
        {
            mode = "teleport";
            golfClub.SetActive(false);
            return;
        }
        if (mode == "teleport")
        {
            mode = "putt";
            laser.enabled = false;
            indicator.SetActive(false);
            ballFollowLaser = false;
            golfClub.SetActive(true);
        }
    }

    private void BallFollowLaser(bool val)
    {
        if (val == true)
        {
            ballFollowLaser = true;
            golfBall.GetComponent<SphereCollider>().enabled = false;
            golfBall.GetComponent<Rigidbody>().useGravity = false;
            Debug.Log("Collider off");
            //TO DO: Add layer mask to ignore ball
        }
        else
        {
            ballFollowLaser = false;
            golfBall.GetComponent<SphereCollider>().enabled = true;
            golfBall.GetComponent<Rigidbody>().useGravity = true;
            Debug.Log("Collider on");
            //TO DO: Remove layer mask to ignore ball
        }
    }
}