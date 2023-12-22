﻿using System;
using System.Linq;
using UnityEngine;

public class StoveCounter : BaseCounter
{
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    private State currentState = State.Idle;

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private float fryingTimer = 0;
    FryingRecipeSO fryingRecipeSO;

    private float burningTimer = 0;
    BurningRecipeSO burningRecipeSO;


    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State State;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (currentState)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    if (fryingTimer >= fryingRecipeSO.fryingTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKithenObject(fryingRecipeSO.output, this);
                        currentState = State.Fried;
                        fryingTimer = 0;
                        burningRecipeSO = GetBurningRecipeSOWithInput(fryingRecipeSO.output);
                        OnStateChanged.Invoke(this, new OnStateChangedEventArgs { State = currentState });
                    }
                    break;
                case State.Fried:
                    fryingTimer += Time.deltaTime;
                    if (fryingTimer >= burningRecipeSO.burningTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKithenObject(burningRecipeSO.output, this);
                        currentState = State.Burned;
                        OnStateChanged.Invoke(this, new OnStateChangedEventArgs { State = currentState });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }


    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            if (this.HasKitchenObject() && !player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                currentState = State.Idle;
                OnStateChanged.Invoke(this, new OnStateChangedEventArgs { State = currentState });
            }
        }
        else
        {
            fryingTimer = 0;

            if (player.HasKitchenObject())
            {
                if (!HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectScriptableObject()))
                {
                    return;
                }

                player.GetKitchenObject().SetKitchenObjectParent(this);
                fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectScriptableObject());
                currentState = State.Frying;
                OnStateChanged.Invoke(this, new OnStateChangedEventArgs { State = currentState });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        return GetFryingRecipeSOWithInput(kitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        return fryingRecipeSOArray.FirstOrDefault(a => a.input == kitchenObjectSO).output;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        return fryingRecipeSOArray.FirstOrDefault(a => a.input == kitchenObjectSO);
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        return burningRecipeSOArray.FirstOrDefault(a => a.input == kitchenObjectSO);
    }
}