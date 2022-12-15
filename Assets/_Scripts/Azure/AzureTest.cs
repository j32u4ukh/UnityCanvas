using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityExpansion;

public class AzureTest : MonoBehaviour
{
    string group_id;
    string group_name;

    // Start is called before the first frame update
    void Start()
    {
        Azure.initConfigData();

        /* 5ce45c41-d387-4afc-8241-279da096e27e, 
         * 9fecd399-d395-488d-be66-7123165fcd9e, 
         * b7b2f64c-2653-4285-beb1-80282fc71e99, 
         * c5c2dc65-8bb4-4da8-9da3-707e5184b8a7, 
         * group_id_is_miyu, 
         * noute_and_miyu_group
         */
        group_id = "sb_group";
        //group_id = "noute_and_miyu_group";
        group_name = "sb";

        string test_item = "postFaceIdentify";

        switch (test_item)
        {
            case "getPersonGroupList":
                _ = getPersonGroupList();
                break;
            case "getPersonList":
                _ = getPersonList(group_id: group_id);
                break;
            case "createPersonGroup":
                _ = createPersonGroup(group_id: group_id, group_name: group_name, "sb1", "sb2");
                break;
            case "buildPeopleMap":
                buildPeopleMap("sb1", "sb2");
                break;
            case "appendPersonIntoGroup":
                Dictionary<string, List<string>> people = new Dictionary<string, List<string>>()
                {
                    { "noute", new List<string>(){ @"D:\Unity Projects\noute8.jpg" } },
                    { "annri", new List<string>()
                        {
                            @"D:\Unity Projects\annri1.jpg",
                            @"D:\Unity Projects\annri2.jpg",
                            @"D:\Unity Projects\annri3.jpg",
                            @"D:\Unity Projects\annri4.jpg",
                            @"D:\Unity Projects\annri5.jpg"
                        }
                    }
                };
                _ = appendPersonIntoGroup(group_id: group_id, people: people);
                break;
            case "postPersonDictionaryCreate":
                _ = postPersonDictionaryCreate(group_id: group_id, "sb1", "sb2");
                break;
            case "deletePersonGroupDelete":
                _ = deletePersonGroupDelete(group_id: group_id);
                break;
            case "postFaceDetect":
                //_ = postFaceDetect(path: Path.Combine(Application.streamingAssetsPath, "AzureImage", "noute", "noute2.jpg"));
                _ = postFaceDetect(path: Path.Combine(@"D:\NTNU\Somatosensory", "婉婷", "sb1 (5).jpg"));
                //_ = postFaceDetect(path: Path.Combine(@"D:\NTNU\Somatosensory", "偉寧", "sb2 (5).jpg"));
                break;
            case "postFaceIdentify":
                //_ = postFaceIdentify(group_id: group_id, file_name: Path.Combine(@"D:\NTNU\Somatosensory", "婉婷", "sb1 (5).jpg"));
                _ = postFaceIdentify(group_id: group_id, file_name: Path.Combine(@"D:\NTNU\Somatosensory", "偉寧", "sb2 (5).jpg"));
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            postFaceDetectCamera();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            postFaceIdentifyCamera();
        }
    }

    #region Detect
    void postFaceDetectCamera()
    {
        Debug.Log("開始: 感測器辨識測試");
        _ = postFaceDetect("orbbec");
        Debug.Log("結束: 感測器辨識測試");
    }

    void postFaceIdentifyCamera()
    {
        Debug.Log("開始: 感測器辨識測試");
        _ = postFaceIdentify(group_id: group_id, file_name: "orbbec");
        Debug.Log("結束: 感測器辨識測試");
    }

    /// <summary>
    /// 偵測並返回圖片中的人臉及位置 by UnityWebRequest
    /// </summary>
    /// <param name="path">要偵測的資源的路徑(若為檔案則須含副檔名；若為 orbbec 則抓取感測器的畫面)</param>
    /// <returns></returns>
    async Task postFaceDetect(string path)
    {
        FaceDetects face_detects = await Azure.postFaceDetect(path);
        Debug.Log(face_detects);
    }

    /// <summary>
    /// 利用 postFaceDetect 偵測到的人臉，與 PersonGroup 以訓練的數據比對，判別是'誰的臉'
    /// </summary>
    /// <param name="group_id"></param>
    /// <param name="file_name"></param>
    /// <returns></returns>
    async Task postFaceIdentify(string group_id, string file_name)
    {
        List<Person> people = await Azure.postFaceIdentify_(group_id, file_name);

        foreach (Person person in people)
        {
            Debug.Log(person);
        }
    }
    #endregion

    #region PersonGroup
    async Task createPersonGroup(string group_id, string group_name, params string[] names)
    {
        Dictionary<string, List<string>> people = Azure.buildPeopleMap(names);

        await Azure.createPersonGroup(group_id: group_id, group_name: group_name, people: people);
    }

    void buildPeopleMap(params string[] names)
    {
        Dictionary<string, List<string>> people = Azure.buildPeopleMap(names);

        foreach(string key in people.Keys)
        {
            Debug.Log(string.Format("key: {0}, value: {1}", key, UnityExpansion.List.ToString(people[key])));
        }
    }

    async Task appendPersonIntoGroup(string group_id, Dictionary<string, List<string>> people)
    {
        await Azure.appendPersonIntoGroup(group_id: group_id, people: people);
    }

    /// <summary>
    /// 取得 PersonGroup 清單，以 PersonGroupList 的形式回傳
    /// </summary>
    /// <returns></returns>
    async Task getPersonGroupList()
    {
        PersonGroupList list = await Azure.getPersonGroupList();
        Debug.Log(list);        
    }

    /// <summary>
    /// 取得 PersonGroup 當中的所有 Person
    /// </summary>
    /// <param name="group_id"></param>
    /// <returns></returns>
    async Task getPersonList(string group_id)
    {
        PersonList list = await Azure.getPersonList(group_id);
        Debug.Log(list);
    }

    /// <summary>
    /// 刪除 PersonGroup
    /// </summary>
    /// <param name="group_id"></param>
    /// <returns></returns>
    async Task deletePersonGroupDelete(string group_id)
    {
        await Azure.deletePersonGroupDelete(group_id);
    }
    #endregion

    #region Person
    async Task postPersonDictionaryCreate(string group_id, params string[] names)
    {
        Dictionary<string, List<string>> people = Azure.buildPeopleMap(names);

        foreach(string name in people.Keys)
        {
            Debug.Log(name);
        }

        //await Azure.postPersonDictionaryCreate(group_id: group_id, people: people);
    }

    /// <summary>
    /// 刪除 Person
    /// </summary>
    /// <param name="group_id"></param>
    /// <param name="person_id"></param>
    /// <returns></returns>
    async Task deletePersonDelete(string group_id, string person_id)
    {
        await Azure.deletePersonDelete(group_id, person_id);
    } 
    #endregion
}
