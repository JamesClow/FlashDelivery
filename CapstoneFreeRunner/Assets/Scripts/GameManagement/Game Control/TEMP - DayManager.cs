using UnityEngine;
using System.Collections;

public class DayManagerTEMP : MonoBehaviour {

    //WHENEVER YOU WANT TO ADD A NEW DAY, MAKE SURE TO SET UP ITS GAMEOBJECT, THEN GO TO (2)
    public GameObject day1;
    public GameObject day2;
    public GameObject day3;
    public GameObject[] days;
    public GameObject hero;
    public GameObject mainCamera;
    private ArrayList rooms;

    private bool spawningRooms = false;
    private int dayIterator = 0;
    private Vector3 maxRoomSize = new Vector3(1, 1, 1);
    private Vector3 roomScaleInc = new Vector3(0.1f, 0.1f, 0.1f);

    public float timer;
    public bool paused;

    public static DayManager self;

    //The day is offset by 1 because we need it to be 0-based to index the days array, but 1-based to index the... human English non-bullshit sensors.
    public int currentDay;
    public int[] mailboxesPerDay;

    void Start() {
        Application.targetFrameRate = 60;
        timer = 0;

        //(2): ADD NEXT DAY TO THIS LIST
        days = new GameObject[] { day1, day2, day3 };

        mailboxesPerDay = new int[days.Length];

        for (int i = 0; i < mailboxesPerDay.Length; i++) {
            mailboxesPerDay[i] = days[i].GetComponentsInChildren<Mailbox>().Length;
        }
        currentDay = 0;
        //loadRooms(days[currentDay]);
        //spawnRooms();

    }

    public void Update() {
        if (!paused) {
            timer += Time.deltaTime;
        }
        if (spawningRooms) {
            if (dayIterator < rooms.Count) {
                if (((GameObject)rooms[dayIterator]).transform.localScale.x < maxRoomSize.x) {
                    ((GameObject)rooms[dayIterator]).transform.localScale += roomScaleInc;
                } else {
                    dayIterator++;
                }
            } else {
                spawningRooms = false;
                dayIterator = 0;
            }
        }
    }

    public bool loadRooms(GameObject day) {
        rooms = new ArrayList();
        foreach (Transform obj in day.transform) {
            if (obj.tag == "RoomContainer") {
                foreach (Transform room in obj) {
                    if (room.tag == "Room") {
                        rooms.Add(room.gameObject);
                    }
                }
                foreach (GameObject room in rooms) {
                    room.transform.localScale = new Vector3(0, 0, 0);
                }
                return true;
            }
        }
        return false;
    }

    public void spawnRooms() {
        spawningRooms = true;
    }

    public void LoadNextDay(GameObject obj) {
        LoadDay(obj, currentDay + 1);
    }

    public void LoadDay(GameObject office, int day) {

        days[day].transform.position += office.transform.localPosition + days[currentDay].transform.position;

        days[day].SetActive(true);
        days[currentDay].SetActive(false);
        currentDay = day;
        StatsTracker.self.ResetDelivered();
        loadRooms(days[day]);
        spawnRooms();
    }

    public bool PaperRouteFinished() {
        return StatsTracker.papersDelivered >= mailboxesPerDay[currentDay];
    }

    public void ResetLevel() {
        //Sends the "Reset" message to all objects in the scene.
        //Every game object should implement a Reset function to reset its position, health,
        // etc for when the player dies or resets the level.
        //days[currentDay].BroadcastMessage ("Reset");
        BroadcastMessage("Reset");
        hero.BroadcastMessage("Reset");
        mainCamera.BroadcastMessage("Reset");
    }

    public void PauseGame(bool pause) {
        paused = pause;
        PlayerController.PlayerInputEnabled = !pause;
    }
}
