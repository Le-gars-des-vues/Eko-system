using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Pour classer des donnes en ordre de priorite
public class Heap<T> where T : IHeapItem<T>
{
    //Array d'item
    T[] items;
    //Nombre d'item dans l'array
    int currentItemCount;

    //Constructeur de la structure 
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    //Methode pour ajouter des items a l'array
    public void Add(T item)
    {
        //L'index de l'item est egale au nombre d'item dans l'array (si on ajoute un items et qu'il y en a 8 dans l'array, son index est 8 car l'array commence a 0
        item.HeapIndex = currentItemCount;
        //L'item est ajouter a la position de son index
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    //On call seulement sortUp car dans le pathfinding les nodes peuvent seulement gagner en priorite
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    //Methode pour comparer les donnees et les mettres plus bas dans la structure
    void SortDown(T item)
    {
        //Continu de rouler jusqu'a ce qu'on break
        while (true)
        {
            //Index des deux enfants dans un arbre de donnees binaire
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            //Variable pour conserver temporairement l'index d'un item si on le change de place
            int swapIndex = 0;

            //Si l'index de l'enfant a gauche est plus petit que le nombre d'element dans l'array
            if (childIndexLeft < currentItemCount)
            {
                //On assigne l'index a swapIndex
                swapIndex = childIndexLeft;
                //Meme chose avec l'enfant de droite
                if (childIndexRight < currentItemCount)
                {
                    //Si l'index de l'enfant de gauche est plus petit que celui de l'enfant de droite (il vient avant dans l'array donc)
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                //Si l'index de l'item a sort est plus petit que l'un des index de ses enfant, on les change de place
                if (item.CompareTo(items[swapIndex]) < 0)
                    Swap(item, items[swapIndex]);
                else
                    return;
            }
            else
                return;
        }
    }

    //Methode pour comparer les donnees et les mettres plus haut dans la structure
    void SortUp(T item)
    {
        //L'index d'un parent devrait toujours etre au minimum le double d'un enfant (-1 pour les nombres impaires)
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            //Si l'index de l'item est plus eleve que son parent on les swap
            T parentItem = items[parentIndex];
            if (item.CompareTo(items[parentIndex]) > 0)
                Swap(item, parentItem);
            else
                break;
        }

        parentIndex = (item.HeapIndex - 1) / 2;
    }

    //Methode pour echanger la place de deux items et echanger leur index respectifs
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

//Utiliser pour comparer les donnes en fonction d'un index (lequel a le plus eleve)
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
