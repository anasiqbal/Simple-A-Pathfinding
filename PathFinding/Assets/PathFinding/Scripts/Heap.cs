﻿using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{
	T[] items;
	int currentItemCount;

	public int Count { get { return currentItemCount; }}

	public Heap(int _maxHeapSize)
	{
		items = new T[_maxHeapSize];
	}

	public void Add(T _item)
	{
		_item._HeapIndex = currentItemCount;
		items [currentItemCount] = _item;
		SortUp(_item);
		currentItemCount++;
	}

	public T RemoveFirst()
	{
		T firstItem = items [0];
		currentItemCount--;

		items [0] = items [currentItemCount];
		items [0]._HeapIndex = 0;

		SortDown(items [0]);
		return firstItem;
	}

	public void UpdateItem(T _item)
	{
		SortUp(_item);
	}

	public bool Contains(T _item)
	{
		return Equals(items [_item._HeapIndex], _item);
	}

	void SortUp(T _item)
	{
		int parentIndex = (_item._HeapIndex - 1) / 2;
		while(true)
		{
			T parentItem = items [parentIndex];
			if(_item.CompareTo(parentItem) > 0) {
				Swap(_item, parentItem);
			}
			else {
				break;
			}

			parentIndex = (_item._HeapIndex - 1) / 2;
		}
	}

	void SortDown(T _item)
	{
		while(true)
		{
			int childIndexLeft = _item._HeapIndex * 2 + 1;
			int childIndexRight = _item._HeapIndex * 2 + 2;

			int swapIndex = 0;

			if (childIndexLeft < currentItemCount)
			{
				swapIndex = childIndexLeft;
				if(childIndexRight < currentItemCount)
				{
					if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
						swapIndex = childIndexRight;
				}

				if(_item.CompareTo(items[swapIndex]) < 0) {
					Swap(_item, items [swapIndex]);
				}
				else {
					return;
				}
			}
			else
			{
				return;
			}
		}
	}

	void Swap(T _itemA, T _itemB)
	{
		items [_itemA._HeapIndex] = _itemB;
		items [_itemB._HeapIndex] = _itemA;

		int itemAIndex = _itemA._HeapIndex;
		_itemA._HeapIndex = _itemB._HeapIndex;
		_itemB._HeapIndex = itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T>
{
	int _HeapIndex { get; set; }
}