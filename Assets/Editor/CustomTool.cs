using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class CustomTool : EditorWindow
{
    private bool useSpecificSetting = false;
    private static Transform startPoint;
    private CustomObject objectToInteractWith;

    private bool useSceneTemplate;
    private GameObject sceneTemplate;

    [MenuItem("Advanced/CustomTool")]
    public static void ShowWindow()
    {
        GetWindow<CustomTool>("Custom Tool");
    }

    /**
     * Устанавливаем начальные в редакторе значения для того, чтобы избежать ручного поиска
     */
    private void Awake()
    {
        // задаём стартовую позицию с помощью поиска пустого GameObject с заданным именем (допустим, такой объект уже есть на вашей сцене)
        startPoint=GameObject.Find("StartPoint")?.transform;
        
        // загружаем объекты, которые хотим инициализировать на сцене, если они отсутствуют
        sceneTemplate = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Prefabs/SceneTemplate.prefab", typeof(GameObject));
        
        // заранее ищем компонент на сцене вместо того, чтобы вручную его выбирать через инспектор
        objectToInteractWith = FindObjectOfType<CustomObject>();

        // если такой компонент отсутствует, но очень нам нужен обычно - создаём его
        if (!objectToInteractWith)
        {
            // создаём новый пустой объект для создания иерархии (это необязательно)
            var objectRoot = new GameObject("|----- ROOT -----|");
            
            // берем нужный нам объект из наших ассетов
            var objectToInstantiate = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Prefabs/CustomObject.prefab", typeof(GameObject));
            
            // добавляем в него объект, которого нам не хватает на сцене, и инициализируем нужно нам поле по умолчанию
            objectToInteractWith = Instantiate(objectToInstantiate, objectRoot.transform).GetComponentInChildren<CustomObject>();
        }
    }

    private void OnGUI ()
    {
        EditorGUIUtility.labelWidth = 250;   
        GUILayout.Label("Настройка сцены", EditorStyles.boldLabel);

        useSceneTemplate = EditorGUILayout.Toggle("Использовать шаблон сцены", useSceneTemplate);
        if(useSceneTemplate)
            sceneTemplate = (GameObject) EditorGUILayout.ObjectField("Шаблон сцены", sceneTemplate, typeof(GameObject), false);
        
        GUILayout.Label("Настройка объектов", EditorStyles.boldLabel);
        
        startPoint= (Transform) EditorGUILayout.ObjectField("Стартовая позиция", startPoint, typeof(Transform), true);
        objectToInteractWith = (CustomObject) EditorGUILayout.ObjectField("Объект, с которым взаимодействовать", objectToInteractWith, typeof(CustomObject), true);
        useSpecificSetting = EditorGUILayout.Toggle("Использовать специальную настройку", useSpecificSetting);

        if (GUILayout.Button("Apply"))
        {
            ApplySettings();
        }
    }

    private void ApplySettings ()
    {
        if(useSceneTemplate)
            Instantiate(sceneTemplate);
        
        // применение настроек только к выбранным объектам
        if (Selection.gameObjects.Length>0)
        {
            foreach (var gameObject in Selection.gameObjects)
                ApplySettingsToSpecificObject(gameObject);
        }
        else
        {
            // применение настроек ко всем объектам на сцене
            var sceneGameObjects = new List<GameObject>();
            var currentScene=SceneManager.GetActiveScene();
            currentScene.GetRootGameObjects(sceneGameObjects);

            foreach (var gameObject in sceneGameObjects)
                ApplySettingsToSpecificObject(gameObject);
        }
    }

    private void ApplySettingsToSpecificObject(GameObject objectToChange)
    {
        // назначаем определённый tag объекту (это пример того случая, если вам нужно какую-то обязательную настройку сделать для нового объекта)
        objectToChange.tag = UnityEditorInternal.InternalEditorUtility.tags[7];
        
        // назначаём стартовую позицию (в качестве альтернативы можно прописать координаты)
        if(startPoint)
            objectToChange.transform.position = startPoint.position;

        // устанавливаем определённую настройку для конкретного скрипта
        var script = objectToChange.GetComponentInChildren<CoolScript>(true);
        if (script)
        {
            script.isSettingActive = useSpecificSetting;
            script.objectToInteractWith = objectToInteractWith;
        }
    }
}