using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityExpansion;

public enum AzureService
{
    [Description("detect")]
    Detect,
    [Description("persongroups")]
    PersonGroupList,
    [Description("identify")]
    Identify,
    [Description("persongroups")]
    Person,
    [Description("persongroups")]
    CreatePerson,
    [Description("persongroups")]
    PersonAddFace,
    [Description("persongroups")]
    TrainPersonGroup,
    [Description("persongroups")]
    GetTrainingStatus,
    [Description("persongroups")]
    DeletePerson,
    [Description("persongroups")]
    PersonList,
    [Description("persongroups")]
    PersonGroup,
    [Description("persongroups")]
    DeletePersonGroup,
}

public static class Azure
{
    public const string RecognitionModel01 = "recognition_01";
    public const string RecognitionModel02 = "recognition_02";
    public const string RecognitionModel03 = "recognition_03";
    public const string DetectionModel01 = "detection_01";
    public const string DetectionModel02 = "detection_02";

    public static string CONFIG_PATH = @"D:\UnityProjects\AzureFaceConfig.txt";

    // 金鑰
    public static string FACE_SUBSCRIPTION_KEY1;
    public static string FACE_SUBSCRIPTION_KEY2;

    // 服務端點
    public static string FACE_ENDPOINT;

    public static void initConfigData(string config_path = "")
    {
        if (!config_path.Equals(""))
        {
            CONFIG_PATH = config_path;
        }

        using (StreamReader reader = new StreamReader(CONFIG_PATH))
        {
            string line;
            string[] content;

            while ((line = reader.ReadLine()) != null)
            {
                content = line.Split(' ');

                switch (content[0])
                {
                    case "FACE_SUBSCRIPTION_KEY1":
                        FACE_SUBSCRIPTION_KEY1 = content[1];
                        break;
                    case "FACE_SUBSCRIPTION_KEY2":
                        FACE_SUBSCRIPTION_KEY2 = content[1];
                        break;
                    case "FACE_ENDPOINT":
                        FACE_ENDPOINT = content[1];
                        break;
                    default:
                        Debug.LogError(string.Format("[Azure] loadConfigData | key: {0}, value: {1}", content[0], content[1]));
                        break;
                }
            }
        }
    }

    public static string newGroupId()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// 取得 API 所需的 bytes 數據
    /// </summary>
    /// <param name="path">要偵測的資源的名稱(若為檔案則須含副檔名；若為 orbbec 則抓取感測器的畫面)</param>
    /// <returns></returns>
    public static async Task<byte[]> getImageBytes(string path)
    {
        Debug.Log(path);
        byte[] bytes;

        using (FileStream file_stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            bytes = new byte[file_stream.Length];
            await file_stream.ReadAsync(bytes, 0, (int)file_stream.Length);
        }

        return bytes;
    }

    /// <summary>
    /// 取得 PersonGroup 清單，以 PersonGroupList 的形式回傳
    /// </summary>
    /// <returns></returns>
    public static async Task<PersonGroupList> getPersonGroupList()
    {
        QueryParameter query = new QueryParameter(service: AzureService.PersonGroupList);
        query.add("top", "1000");
        query.add("returnRecognitionModel", "false");

        using (UnityWebRequest request = UnityWebRequest.Get(uri: query.ToString()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);

            await request.SendWebRequest();

            // Get the JSON response.
            string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                json_string = request.downloadHandler.text;
                PersonGroupList list = PersonGroupList.loadData(json_string);
                return list;
            }
        }

        return null;
    }

    /// <summary>
    /// 偵測並返回圖片中的人臉及位置 by UnityWebRequest
    /// </summary>
    /// <param name="path">要偵測的資源的路徑(若為檔案則須含副檔名；若為 orbbec 則抓取感測器的畫面)</param>
    /// <returns></returns>
    public static async Task<FaceDetects> postFaceDetect(string path)
    {
        QueryParameter query = new QueryParameter(service: AzureService.Detect);
        query.add("returnFaceId", true);
        query.add("returnFaceLandmarks", false);
        query.add("recognitionModel", RecognitionModel01);
        query.add("returnRecognitionModel", false);
        query.add("detectionModel", DetectionModel01);

        using (UnityWebRequest request = UnityWebRequest.Post(uri: query.ToString(), formData: new WWWForm()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);
            request.SetRequestHeader("Content-Type", "application/octet-stream");

            byte[] bytes = await getImageBytes(path);
            request.uploadHandler.contentType = "application/octet-stream";
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            // Get the JSON response.
            string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                json_string = request.downloadHandler.text;
                FaceDetects face_detects = FaceDetects.loadData(json_string);
                return face_detects;
            }
        }

        return null;
    }

    /// <summary>
    /// 若該 Person 存在於此 PersonGroup，則返回該 Person
    /// </summary>
    /// <param name="group_id">PersonGroup 的唯一對應碼</param>
    /// <param name="person_id">Person 的唯一對應碼</param>
    /// <returns></returns>
    public static async Task<Person> getPerson(string group_id, string person_id)
    {
        QueryParameter query = new QueryParameter(service: AzureService.Person, group_id: group_id, person_id: person_id);

        using (UnityWebRequest request = UnityWebRequest.Get(uri: query.ToString()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            // Get the JSON response.
            string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                json_string = request.downloadHandler.text;
                Person person = JsonConvert.DeserializeObject<Person>(json_string);
                return person;
            }
        }

        return null;
    }

    public static async Task<List<Person>> postFaceIdentify_(string group_id, string file_name, int tps = 3000, int n_attempt_limit = 5)
    {
        FaceDetects face_detects = new FaceDetects();
        int attempt = 0;

        // n_attempt: 設定嘗試次數上限，避免一直重複嘗試
        while (face_detects.getDetectedNumber() == 0 && attempt < n_attempt_limit)
        {
            face_detects = await postFaceDetect(file_name);
            attempt++;
            Debug.Log(string.Format("嘗試第 {0} 次", attempt));

            if (tps > 0)
            {
                // Limit TPS (避免請求頻率過高) 3000
                Debug.Log(string.Format("Limit TPS"));
                await Task.Delay(tps);
            }
        }

        Debug.Log(face_detects);

        if (face_detects == null || face_detects.getDetectedNumber() == 0)
        {
            Debug.Log("請再重新辨識一次");
            return null;
        }
        else
        {
            List<Person> people = await postFaceIdentify(group_id: group_id, face_detects: face_detects, tps: tps);
            return people;
        }
    }

    public static async Task<List<Person>> postFaceIdentify(string group_id, FaceDetects face_detects, int tps = 3000)
    {
        QueryParameter query = new QueryParameter(service: AzureService.Identify);
        Debug.Log(string.Format("uri: {0}", query.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Post(uri: query.ToString(), formData: new WWWForm()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);
            request.SetRequestHeader("Content-Type", "application/json");

            IdentifyRequestBody request_body = new IdentifyRequestBody
            {
                personGroupId = group_id
            };
            request_body.add(face_detects.getFaceIds());

            string json_data = JsonConvert.SerializeObject(request_body);
            byte[] bytes = Encoding.UTF8.GetBytes(json_data);

            request.uploadHandler.contentType = "application/json";
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            // Get the JSON response.
            string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                json_string = request.downloadHandler.text;

                MultiFaceIdentify multi_identify = MultiFaceIdentify.loadData(json_string);
                Debug.Log(multi_identify.ToString());

                List<string> ids = multi_identify.getPersonIds();
                Debug.Log(string.Format("辨識出 {0} 張資料庫中的人臉", ids.Count));
                List<Person> people = new List<Person>();
                Person person;

                foreach (string person_id in ids)
                {
                    person = await getPerson(group_id, person_id);
                    people.Add(person);
                    Debug.Log(person.ToString());

                    if (tps > 0)
                    {
                        // Limit TPS (避免請求頻率過高) 3000
                        Debug.Log(string.Format("Limit TPS"));
                        await Task.Delay(tps);
                    }
                }

                Debug.Log(string.Format("辨識結束"));
                return people;
            }
        }

        return null;
    }

    #region Create PerseonGroup
    // userData 既然是 (optional) 那就表示它並非創建時所必要的
    public static async Task createPersonGroup(string group_id, string group_name, Dictionary<string, List<string>> people, int tps = 3000)
    {
        Debug.Log(string.Format("開始建立 PerseonGroup(group_id: {0}, group_name: {1})", group_id, group_name));
        Debug.Log(string.Format("開始初始化"));
        await putPerseonGroupCreate(group_id: group_id, group_name: group_name);

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        await postPersonDictionaryCreate(group_id: group_id, people: people, tps: tps);

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        await postPersonAddFace_(group_id: group_id, people: people, tps: tps);

        //// 取得 PersonList，用於獲得各個 Person 的 person_id
        //PersonList list = await getPersonList(group_id);
        //string person_id;
        //bool found_person;

        //foreach (Person person in list.people)
        //{
        //    found_person = false;

        //    foreach (string name in people.Keys)
        //    {
        //        // 根據 name 取得 Person，再利用 Person 取得 personId
        //        if (name.Equals(person.name))
        //        {
        //            Debug.LogWarning(string.Format("開始 Person {0} 照片添加", person.name));
        //            found_person = true;
        //            person_id = person.personId;

        //            foreach(string source_name in people[name])
        //            {
        //                await postPersonAddFace(group_id: group_id, person_id: person_id, source_name: source_name);

        //                if (tps > 0)
        //                {
        //                    // Limit TPS (避免請求頻率過高) 3000
        //                    Debug.Log(string.Format("Limit TPS"));
        //                    await Task.Delay(tps);
        //                }
        //            }

        //            break;
        //        }
        //    }

        //    if (found_person)
        //    {
        //        Debug.LogWarning(string.Format("完成 Person {0} 照片添加", person.name));
        //    }
        //    else
        //    {
        //        Debug.LogWarning(string.Format("Person {0} 沒有找到", person.name));
        //    }
        //}

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        await postPersonGroupTrain(group_id: group_id);

        Debug.Log(string.Format("完成建立 PerseonGroup(group_id: {0}, group_name: {1})", group_id, group_name));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="group_id"></param>
    /// <param name="group_name"></param>
    /// <param name="people">新舊成員的名稱與對應的圖片路徑，未排除重複圖片，無法像 putPerseonGroupCreate 直接讀取資料夾，應自行建立</param>
    /// <param name="tps"></param>
    /// <returns></returns>
    public static async Task appendPersonIntoGroup(string group_id, Dictionary<string, List<string>> people, int tps = 3000)
    {
        Debug.Log(string.Format("開始追加數據至 PersonGroup((group_id: {0})", group_id));

        // 取得 PersonList，用於避免重複創建成員
        PersonList list = await getPersonList(group_id);

        List<string> name_list = new List<string>();

        foreach (Person person in list.people)
        {
            name_list.Add(person.name);
        }

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        await postPersonDictionaryCreate(group_id: group_id, people: people, tps: tps, name_list: name_list);

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        await postPersonAddFace_(group_id: group_id, people: people, tps: tps);

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        await postPersonGroupTrain(group_id: group_id);

        Debug.Log(string.Format("完成 PerseonGroup(group_id: {0}) 數據追加與再訓練", group_id));
    }

    public static Dictionary<string, List<string>> buildPeopleMap(params string[] names)
    {
        Dictionary<string, List<string>> people = new Dictionary<string, List<string>>();
        string dir;
        string[] files;

        foreach (string name in names)
        {
            dir = Path.Combine(Application.streamingAssetsPath, "AzureImage", name);

            if (Directory.Exists(dir))
            {
                people.Add(name, new List<string>());
                files = Directory.GetFiles(dir);

                foreach (string file in files)
                {
                    if (!file.EndsWith(".meta"))
                    {
                        people[name].Add(file);
                        Debug.Log(file);
                    }
                }
            }
            else
            {
                Debug.LogWarning(string.Format("沒有資料夾 {0}", dir));
            }
        }

        return people;
    }

    /// <summary>
    /// 建立 PersonGroup(空殼，只有名稱)
    /// </summary>
    /// <param name="group_id">PersonGroup 的唯一對應碼</param>
    /// <param name="group_name">PersonGroup 名稱</param>
    /// <returns></returns>
    public static async Task putPerseonGroupCreate(string group_id, string group_name)
    {
        QueryParameter query = new QueryParameter(service: AzureService.PersonGroup, group_id: group_id);

        PersonGroupRequestBody body = new PersonGroupRequestBody()
        {
            name = group_name
        };

        Debug.Log(string.Format("PersonGroupRequestBody: {0}", body.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Put(uri: query.ToString(), bodyData: body.toBytes()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY2);
            request.SetRequestHeader("Content-Type", "application/json");

            request.uploadHandler.contentType = "application/json";
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log(string.Format("成功建立 PerseonGroup, name: {0}, group_id: {1}", group_name, group_id));
            }
        }
    }

    /// <summary>
    /// 創建新成員至 PerseonGroup 當中，已存在成員不會被重複創建
    /// </summary>
    /// <param name="group_id"></param>
    /// <param name="people"></param>
    /// <param name="tps"></param>
    /// <param name="name_list"></param>
    /// <returns></returns>
    public static async Task postPersonDictionaryCreate(string group_id, Dictionary<string, List<string>> people, int tps = 3000, List<string> name_list = null)
    {
        if (name_list == null)
        {
            name_list = new List<string>();
        }

        bool is_unique;

        foreach (string person_name in people.Keys)
        {
            is_unique = true;

            // 利用 name_list 避免建立重複的成員
            foreach (string name in name_list)
            {
                if (person_name.Equals(name))
                {
                    Debug.Log(string.Format("Person({0}) 已存在，將不重複創建", name));
                    is_unique = false;
                    break;
                }
            }

            if (is_unique)
            {
                Debug.Log(string.Format("開始建立 Person: {0}", person_name));
                await postPersonCreate(group_id: group_id, person_name: person_name);
                Debug.Log(string.Format("完成建立 Person: {0}", person_name));

                if (tps > 0)
                {
                    // Limit TPS (避免請求頻率過高) 3000
                    Debug.Log(string.Format("Limit TPS"));
                    await Task.Delay(tps);
                }
            }
        }
    }

    public static async Task postPersonCreate(string group_id, string person_name)
    {
        QueryParameter query = new QueryParameter(service: AzureService.CreatePerson, group_id: group_id);
        Debug.Log(string.Format("uri: {0}", query.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Post(uri: query.ToString(), formData: new WWWForm()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY2);
            request.SetRequestHeader("Content-Type", "application/json");

            PersonCreateRequestBody body = new PersonCreateRequestBody()
            {
                name = person_name
            };

            request.uploadHandler.contentType = "application/json";
            request.uploadHandler = new UploadHandlerRaw(body.toBytes());
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            //string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                //json_string = request.downloadHandler.text;
                //Debug.Log(string.Format(string.Format("json_string: {0}", json_string)));
                Debug.Log(string.Format(string.Format("新增 {0} 至 PersonGroup: {1}", person_name, group_id)));
            }
        }
    }

    /// <summary>
    /// 對 postPersonAddFace 進行封裝，進行多人多張臉部圖片的添加
    /// </summary>
    /// <param name="group_id"></param>
    /// <param name="people"></param>
    /// <param name="tps"></param>
    /// <returns></returns>
    public static async Task postPersonAddFace_(string group_id, Dictionary<string, List<string>> people, int tps = 3000)
    {
        PersonList list = await getPersonList(group_id);

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        string person_id;
        bool found_person;

        foreach (Person person in list.people)
        {
            found_person = false;

            foreach (string name in people.Keys)
            {
                // 根據 name 取得 Person，再利用 Person 取得 personId
                if (name.Equals(person.name))
                {
                    Debug.LogWarning(string.Format("開始 Person {0} 照片添加", person.name));
                    found_person = true;
                    person_id = person.personId;

                    foreach (string path in people[name])
                    {
                        await postPersonAddFace(group_id: group_id, person_id: person_id, path: path);

                        if (tps > 0)
                        {
                            // Limit TPS (避免請求頻率過高) 3000
                            Debug.Log(string.Format("Limit TPS"));
                            await Task.Delay(tps);
                        }
                    }

                    break;
                }
            }

            if (found_person)
            {
                Debug.LogWarning(string.Format("完成 Person {0} 照片添加", person.name));
            }
            else
            {
                Debug.LogWarning(string.Format("Person {0} 沒有找到", person.name));
            }
        }
    }

    /// <summary>
    /// 臉部圖片的添加
    /// </summary>
    /// <param name="group_id">PersonGroup 的唯一對應碼</param>
    /// <param name="person_id">Person 的唯一對應碼</param>
    /// <param name="path">要偵測的資源的名稱(若為檔案則須含副檔名；若為 orbbec 則抓取感測器的畫面)</param>
    /// <returns></returns>
    public static async Task postPersonAddFace(string group_id, string person_id, string path)
    {
        QueryParameter query = new QueryParameter(service: AzureService.PersonAddFace, group_id: group_id, person_id: person_id);
        query.add("detectionModel", DetectionModel01);
        //Debug.Log(string.Format("uri: {0}", query.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Post(uri: query.ToString(), formData: new WWWForm()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);
            request.SetRequestHeader("Content-Type", "application/octet-stream");

            byte[] bytes = await getImageBytes(path);
            request.uploadHandler.contentType = "application/octet-stream";
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            // Get the JSON response.
            string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                json_string = request.downloadHandler.text;
                Debug.Log(string.Format("json_string: {0}", json_string));
            }
        }
    }

    public static async Task postPersonGroupTrain(string group_id, int tps = 3000)
    {
        QueryParameter query = new QueryParameter(service: AzureService.TrainPersonGroup, group_id: group_id);
        Debug.Log(string.Format("uri: {0}", query.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Post(uri: query.ToString(), formData: new WWWForm()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);

            request.uploadHandler.contentType = "application/octet-stream";
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            // Get the JSON response.
            //string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                //json_string = request.downloadHandler.text;
                Debug.Log(string.Format("完成 PersonGroup {0} 的訓練", group_id));

                if (tps > 0)
                {
                    // Limit TPS (避免請求頻率過高) 3000
                    Debug.Log(string.Format("Limit TPS"));
                    await Task.Delay(tps);
                }

                string training_status = await getPersonGroupGetTrainingStatus(group_id);
                Debug.Log(string.Format("training_status: {0}", training_status));
            }
        }
    }

    public static async Task<string> getPersonGroupGetTrainingStatus(string group_id)
    {
        QueryParameter query = new QueryParameter(service: AzureService.GetTrainingStatus, group_id: group_id);
        Debug.Log(string.Format("uri: {0}", query.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Get(uri: query.ToString()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);

            await request.SendWebRequest();

            // Get the JSON response.
            string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                json_string = request.downloadHandler.text;
                return json_string;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 根據 group_id 取得該 PersonGroup 當中所有人
    /// </summary>
    /// <param name="group_id">PersonGroup 的唯一對應碼</param>
    /// <returns></returns>
    public static async Task<PersonList> getPersonList(string group_id)
    {
        QueryParameter query = new QueryParameter(service: AzureService.PersonList, group_id: group_id);
        query.add("top", 1000);
        //Debug.Log(string.Format("uri: {0}", query.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Get(uri: query.ToString()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY1);

            await request.SendWebRequest();

            // Get the JSON response.
            string json_string;
            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                json_string = request.downloadHandler.text;
                PersonList list = PersonList.loadData(json_string);
                return list;
            }
        }

        return null;
    }

    /// <summary>
    /// 根據 person_id 與 group_id 刪除對應的 Person
    /// </summary>
    /// <param name="group_id">PersonGroup 的唯一對應碼</param>
    /// <param name="person_id">Person 的唯一對應碼</param>
    /// <returns></returns>
    public static async Task deletePersonDelete(string group_id, string person_id, int tps = 3000)
    {
        QueryParameter query = new QueryParameter(service: AzureService.DeletePerson, group_id: group_id, person_id: person_id);
        Debug.Log(string.Format("uri: {0}", query.ToString()));

        using (UnityWebRequest request = UnityWebRequest.Delete(uri: query.ToString()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY2);

            await request.SendWebRequest();

            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log(string.Format("成功刪除 Person: {0}", person_id));
            }
        }

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        //
        PersonList list = await getPersonList(group_id);
        Debug.Log(list);
    }

    /// <summary>
    /// 根據 group_id 刪除 PersonGroup
    /// </summary>
    /// <param name="group_id">PersonGroup 的唯一對應碼</param>
    /// <returns></returns>
    public static async Task deletePersonGroupDelete(string group_id, int tps = 3000)
    {
        QueryParameter query = new QueryParameter(service: AzureService.PersonGroup, group_id: group_id);

        using (UnityWebRequest request = UnityWebRequest.Delete(uri: query.ToString()))
        {
            // 設置 Header
            // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SetRequestHeader.html
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY2);

            await request.SendWebRequest();

            while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log(string.Format("成功刪除 PersonGroup: {0}", group_id));
            }
        }

        if (tps > 0)
        {
            // Limit TPS (避免請求頻率過高) 3000
            Debug.Log(string.Format("Limit TPS"));
            await Task.Delay(tps);
        }

        PersonGroupList list = await getPersonGroupList();
        Debug.Log(list);
    }
    #endregion
}

public class QueryParameter
{
    private StringBuilder sb;
    private bool initialized;

    public QueryParameter(string service)
    {
        string query = string.Format("{0}/face/v1.0/{1}?", Azure.FACE_ENDPOINT, service);
        sb = new StringBuilder(query);
    }

    public QueryParameter(AzureService service, string group_id = "", string person_id = "")
    {
        string query = string.Format("{0}/face/v1.0/{1}", Azure.FACE_ENDPOINT,
            EnumTool.GetDescription<AzureService>(service));

        switch (service)
        {
            case AzureService.CreatePerson:
            case AzureService.PersonList:
                query = string.Format("{0}/{1}/persons", query, group_id);
                break;
            case AzureService.Person:
            case AzureService.DeletePerson:
                query = string.Format("{0}/{1}/persons/{2}", query, group_id, person_id);
                break;
            case AzureService.PersonAddFace:
                query = string.Format("{0}/{1}/persons/{2}/persistedFaces", query, group_id, person_id);
                break;
            case AzureService.TrainPersonGroup:
                query = string.Format("{0}/{1}/train", query, group_id);
                break;
            case AzureService.GetTrainingStatus:
                query = string.Format("{0}/{1}/training", query, group_id);
                break;
            case AzureService.PersonGroup:
                query = string.Format("{0}/{1}", query, group_id);
                break;
        }

        sb = new StringBuilder(query);
    }

    public void add(string key, string value)
    {
        if (initialized)
        {
            sb.Append("&");
        }
        else
        {
            initialized = true;
            sb.Append("?");
        }

        sb.Append(string.Format("{0}={1}", key, value));
    }

    public void add(string key, bool value)
    {
        if (value)
        {
            add(key, value: "true");
        }
        else
        {
            add(key, value: "false");
        }
    }

    public void add(string key, int value)
    {
        add(key, value.ToString());
    }

    public void addList(string key, params string[] values)
    {
        StringBuilder sb = new StringBuilder("[");

        for (int i = 0; i < values.Length; i++)
        {
            if (i != 0)
            {
                sb.Append(",");
            }

            sb.Append(values[i]);
        }

        sb.Append("]");

        add(key, value: sb.ToString());
    }

    public override string ToString()
    {
        return sb.ToString();
    }

}
