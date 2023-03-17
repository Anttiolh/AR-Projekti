using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public ARRaycastManager manager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public GameObject spawnablePrefab;
    Camera arCam;
    GameObject spawnedObject;
    int numberOfBlocks;
    public Slider slider;
    public GameObject victoryScreen;

    List<Color> colorList = new List<Color>();
    void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").transform.GetComponent<Camera>();
        colorList.Add(Color.red);
        colorList.Add(Color.blue);
        colorList.Add(Color.yellow);
        colorList.Add(Color.black);
        colorList.Add(Color.cyan);
    }

    void Update()
    {
        // Kosketaan näyttöön eikä osuta mihinkään
        if(Input.touchCount == 0)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        // Kosketaan näyttöön ja osutaan johonkin objektiin
        if(manager.Raycast(Input.GetTouch(0).position, hits))
        {
            // Ensikosketuksen aikana
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if(Physics.Raycast(ray,out hit))
                {
                    if(hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                        // Jos olemassa olevaa kuutiota kosketaan, annetaan sille uusi random väri ennalta määritellystä listasta
                        int rng = Random.Range(0, 5);
                        spawnedObject.GetComponent<MeshRenderer>().material.color = colorList[rng];
                    }
                    else
                    {
                        SpawnPrefab(hits[0].pose.position);
                        numberOfBlocks++;
                        slider.value = numberOfBlocks;
                        // Pelin voittoehto
                        if(numberOfBlocks > 9)
                        {
                            victoryScreen.SetActive(true);
                        }
                    }
                }
            }

            // Kun sormea liikutellaan näytöllä
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
            {
                spawnedObject.transform.position = hits[0].pose.position;
            }

            // Kun sormi irroitetaan näytöltä
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                spawnedObject = null;
            }
        }
    }

    void SpawnPrefab(Vector3 position)
    {
        spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);
    }
}
