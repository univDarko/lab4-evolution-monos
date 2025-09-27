using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MonoCollector : MonoBehaviour
{
    [SerializeField] private int contMono = 6;
    [SerializeField] private GameObject monkeyObject;
    [SerializeField] private GameObject bananaObject;

    [SerializeField] private List<GameObject> monos;

    [SerializeField] private int bananas = 10;
    [SerializeField] private readonly float time = 5;
    private float realtime = 0;
    private bool trigger = false;

    [SerializeField] private int mapHeight = 30;
    [SerializeField] private int mapWidth = 30;

    [SerializeField] private int childrenCount = 2;

    [SerializeField] private bool execute = false;

    private void Start()
    {
        for (int i = 0; i < contMono; i++)
        {
            var mono = Instantiate(monkeyObject);
            mono.GetComponent<Renderer>().material.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            monos.Add(mono);
        }

        foreach (var mono in monos)
        {
            mono.transform.position = new Vector3(UnityEngine.Random.Range(0f, mapWidth),
                0f, UnityEngine.Random.Range(0f, mapHeight));

            var monochar = mono.GetComponent<MonoMonkey>();

            monochar.area = UnityEngine.Random.Range(1f, 5f);
            monochar.velocity = UnityEngine.Random.Range(1f, 5f);

            mono.GetComponent<SphereCollider>().radius = mono.GetComponent<MonoMonkey>().area;
        }
    }

    private void Update()
    {
        if (realtime > 0)
        {
            realtime -= Time.deltaTime;
        }
        else if (trigger)
        {
            Eval();
            trigger = false;
        }
    }

    private void LateUpdate()
    {
        if (execute)
        {
            Execute();

            execute = false;
        }
    }

    private void Execute()
    {
        for (int i = 0; i < bananas; i++)
        {
            var ban = Instantiate(bananaObject);
            ban.transform.position = new Vector3(UnityEngine.Random.Range(0f, mapWidth),
                0f, UnityEngine.Random.Range(0f, mapHeight));
        }

        realtime = time;

        foreach (var mono in monos)
        {
            var monochar = mono.GetComponent<MonoMonkey>();

            StartCoroutine(MonoMovement(monochar.velocity, mono));
        }

        trigger = true;
    }

    private IEnumerator MonoMovement(float vel, GameObject obj)
    {
        obj.transform.position = new Vector3(UnityEngine.Random.Range(0, mapWidth),
                0f, UnityEngine.Random.Range(0, mapHeight));

        yield return new WaitForSeconds(vel);

        if (realtime > 0)
            StartCoroutine(MonoMovement(vel, obj));
    }  

    private void Eval()
    {
        List<GameObject> parents = new List<GameObject>();

        var findBest = monos
            .Select(mono => new { mono, banana = mono.GetComponent<MonoMonkey>().bananas }) // Obtenemos el valor de banana
            .OrderByDescending(m => m.banana) // Ordenamos de mayor a menor
            .Select(m => m.mono) // Seleccionamos solo los GameObjects ordenados
            .ToArray(); // Convertimos el resultado en un array

        //mejor usar mitad población
        parents.Add(findBest[0]);
        parents.Add(findBest[1]);

        foreach (var mono in findBest)
        {
            Destroy(mono);
        }
        monos.Clear();


        foreach (var mono in parents)
        {
            var parent = Instantiate(mono);
            monos.Add(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = Instantiate(mono);
                var monochar = child.GetComponent<MonoMonkey>();

                Mutate(monochar);
                mono.GetComponent<Renderer>().material.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

                monos.Add(child);
            }
        }

    }

    private void Mutate(MonoMonkey monochar)
    {
        monochar.velocity += Mathf.Clamp(UnityEngine.Random.Range(-2f, 2f), 0, 10);
        monochar.area += Mathf.Clamp(UnityEngine.Random.Range(-2f, 2f), 0, 10);
    }
}
