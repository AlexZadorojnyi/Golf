using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    //Hole independent vars
    private int holeCount;
    private int totalScore;
    private int totalPars;
    private GameObject[] holes;
    public GameObject flag;
    public GameObject rCont;
    public GameObject lCont;
    public Text finalScoreBoardTotals;
    //Hole specific vars
    private Vector3 ballPos;
    private Vector3 holePos;
    private float holeRadius;
    private int holePar;
    private int holeScore;
    //Do this another way
    private bool inHole;

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        //If past the edge of the hole
        if (Vector3.Distance(transform.position, holePos) < holeRadius)
        {
            //Turn off collider
            GetComponent<SphereCollider>().enabled = false;
            inHole = true;
        }
        //If kinematics are turned off
        if (inHole)
        {
            //Check if ball is still past edge of hole
            if (Vector3.Distance(transform.position, holePos) >= holeRadius)
            {
                inHole = false;
                //Reset collider
                GetComponent<SphereCollider>().enabled = true;
                //Update total score
                totalScore = totalScore + holeScore / 2;
                //Store hole score
                holes[holeCount].GetComponent<Hole>().UpdateScore(holeScore/2);
                Debug.Log(PrintScore());
                //Increment hole count
                holeCount++;
                //Load next hole
                GetHole(holeCount);
                //Reset ball - Moved this to GetHole
                //Reset();
                //Update scoreboard
                finalScoreBoardTotals.text = "Total\n" + totalPars + "\n" + totalScore;
            }
        }

        //DEBUG
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            //Increment hole count
            holeCount++;
            //Load next hole
            GetHole(holeCount);
            //Reset ball
            Reset();
        }

        //DEBUG
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            //Increment hole count
            holeCount--;
            //Load next hole
            GetHole(holeCount);
            //Reset ball
            Reset();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //This runs twice for some reason (holeScore++ at least)
        if (other.tag == "Golf Club Head")
        {
            //Increment hole score
            holeScore++;
            //Transfer velocity
            GetComponent<Rigidbody>().velocity = other.GetComponent<GolfClubHead>().getVelocity() * 1.25F;
            //Despawn flag
            flag.SetActive(false);
            //Vibrate controller
            if(rCont != null) rCont.GetComponent<Hand>().Vibrate();
            if(lCont != null) lCont.GetComponent<Hand>().Vibrate();
            //GameObject.Find("Controller (right)").GetComponent<Hand>().Vibrate();
            //GameObject.Find("Controller (left)").GetComponent<Hand>().Vibrate();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "Border") {
            GetComponent<Rigidbody>().velocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, collision.contacts[0].normal) * 0.75F;
        }
    }

    /**private void OnCollisionStay(Collision collision) {
        if (collision.collider.tag == "Border") {
            Debug.Log("Border");
            GetComponent<Rigidbody>().velocity = Vector3.Reflect(GetComponent<Rigidbody>().velocity, collision.contacts[0].normal);
        }
    }**/

    public void Reset()
    {
        if (holeCount == holes.Length)
        {
            NewGame();
        }
        else
        {
            transform.position = ballPos;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    public void NewGame()
    {
        //Set initial hole count
        holeCount = 0;
        //Set initial total score
        totalScore = 0;
        //Set initial total pars
        totalPars = 0;
        //Find all holes
        holes = GameObject.FindGameObjectsWithTag("Hole").OrderBy(go => go.name).ToArray();
        //DEBUG
        //Debug.Log("Number of holes: " + holes.Length);
        foreach (GameObject hole in holes)
        {
            totalPars = totalPars + hole.GetComponent<Hole>().holePar;
            hole.GetComponent<Hole>().UpdateScore(0);
        }
        finalScoreBoardTotals.text = "Total\n" + totalPars + "\n" + totalScore;
        //Load first hole info
        GetHole(holeCount);
        //Set ball
        Reset();
        inHole = false;
        holeScore = 0;
    }

    public void GetHole(int num)
    {
        if (num >= 0 && num <= holes.Length - 1 && holes[num] != null)
        {
            //Set ball position
            ballPos = holes[num].GetComponent<Hole>().GetBallPos() + new Vector3(0, transform.localScale.y / 2, 0);
            //Set hole position
            holePos = holes[num].transform.position;
            //Set hole radius
            holeRadius = holes[num].transform.localScale.x / 2;
            //Set hole par
            holePar = holes[num].GetComponent<Hole>().holePar;
            //Set hole score
            holeScore = 0;
            //Spawn flag
            flag.transform.position = holePos;
            flag.SetActive(true);
            //Reset ball
            Reset();
            //DEBUG
            //Debug.Log("Hole #: " + holeCount + "\nBall Pos: " + ballPos + "\nHole Pos: " + holePos + "\nHole Par :" + holePar);
        }
    }

    public bool IsInPlay()
    {
        if (holeScore != 0) return true;
        return false;
    }

    private string PrintScore() {
        string Score = "";
        //Hole in one - Ace
        if (holeScore / 2 == 1) {
            Score = "Hole in one!";
        } else {
            float overPar = holeScore / 2 - holePar;
            //Three or more under par
            if (overPar <= -3) {
                Score = overPar + " under par!";
            }
            //Two under par - Eagle
            if (overPar == -2) {
                Score = "2 under par! (Eagle)";
            }
            //One under par - Birdie
            if (overPar == -1) {
                Score = "1 under par! (Birdie)";
            }
            //Par
            if (overPar == 0) {
                Score = "Par.";
            }
            //One over par - Bogey
            if (overPar == 1) {
                Score = "1 over par. (Bogey)";
            }
            //Two over par - Double Bogey
            if (overPar == 2) {
                Score = "2 over par. (Double bogey)";
            }
            //Three or more over par
            if (overPar >= 3) {
                Score = System.Math.Abs(overPar) + " over par.";
            }
        }
        return Score;
    }
}

