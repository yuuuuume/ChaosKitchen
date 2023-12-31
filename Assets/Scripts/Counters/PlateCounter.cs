﻿using System;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    private float spawnPlateTimer = 0;
    private float spawnPlateCount = 0;

    private const float spawnPlateTimerMax = 3f;
    private const float spawnPlateMax = 5;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public EventHandler OnSpawnEvent;
    public EventHandler OnPlateRemovedEvent;

    private void Update()
    {
        if (spawnPlateCount < spawnPlateMax)
        {
            spawnPlateTimer += Time.deltaTime;
            if (spawnPlateTimer > spawnPlateTimerMax)
            {
                OnSpawnEvent?.Invoke(this, EventArgs.Empty);
                spawnPlateTimer = 0;
                spawnPlateCount++;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (spawnPlateCount > 0)
            {
                spawnPlateCount--;
                KitchenObject.SpawnKithenObject(kitchenObjectSO, player);
                OnPlateRemovedEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
