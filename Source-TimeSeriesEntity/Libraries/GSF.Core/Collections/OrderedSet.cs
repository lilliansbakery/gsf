﻿//******************************************************************************************************
//  OrderedSet.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/04/2013 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

namespace GSF.Collections
{
    /// <summary>
    /// Represents an ordered set of data.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    [Serializable]
    public class OrderedSet<T> : IOrderedSet<T>, ISerializable
    {
        #region [ Members ]

        // Nested Types
        private class NodeComparer : IEqualityComparer<LinkedListNode<T>>
        {
            private IEqualityComparer<T> m_equalityComparer;

            public NodeComparer(IEqualityComparer<T> equalityComparer)
            {
                m_equalityComparer = equalityComparer;
            }

            public bool Equals(LinkedListNode<T> x, LinkedListNode<T> y)
            {
                return m_equalityComparer.Equals(x.Value, y.Value);
            }

            public int GetHashCode(LinkedListNode<T> obj)
            {
                return m_equalityComparer.GetHashCode(obj.Value);
            }
        }

        // Fields
        private HashSet<LinkedListNode<T>> m_hashSet;
        private LinkedList<T> m_linkedList;
        private IEqualityComparer<T> m_equalityComparer;

        private int m_lastVisitedIndex;
        private LinkedListNode<T> m_lastVisitedNode;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedSet{T}"/> class that is empty and uses the default equality
        /// comparer for the set type.
        /// </summary>
        public OrderedSet()
            : this(Enumerable.Empty<T>(), EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedSet{T}"/> class that uses the default equality comparer for
        /// the set type, contains elements copied from the specified <paramref name="collection"/>, and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        public OrderedSet(IEnumerable<T> collection)
            : this (collection, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedSet{T}"/> class that is empty and uses the specified
        /// equalilty <paramref name="comparer"/> for the set type.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set, or <c>null</c> to
        /// use the default <see cref="IEqualityComparer{T}"/> implementation for the set type.
        /// </param>
        public OrderedSet(IEqualityComparer<T> comparer)
            : this(Enumerable.Empty<T>(), comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedSet{T}"/> class that uses the specified equality
        /// <paramref name="comparer"/> for the set type, contains elements copied from the specified <paramref name="collection"/>,
        /// and has sufficient capacity to accommodate the number of elements copied. 
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the set, or <c>null</c> to
        /// use the default <see cref="IEqualityComparer{T}"/> implementation for the set type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        public OrderedSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if ((object)collection == null)
                throw new ArgumentNullException("collection");

            m_hashSet = new HashSet<LinkedListNode<T>>(new NodeComparer(comparer));
            m_linkedList = new LinkedList<T>();
            m_equalityComparer = comparer;

            foreach (T item in collection)
                Add(item);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedSet{T}"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo"/> object that contains the information required to serialize the
        /// <see cref="OrderedSet{T}"/> object.
        /// </param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> structure that contains the source and destination of the serialized stream
        /// associated with the <see cref="OrderedSet{T}"/> object.
        /// </param>
        protected OrderedSet(SerializationInfo info, StreamingContext context)
        {
            m_equalityComparer = (IEqualityComparer<T>)info.GetValue("equalityComparer", typeof(IEqualityComparer<T>));
            m_hashSet = new HashSet<LinkedListNode<T>>(new NodeComparer(m_equalityComparer));
            m_linkedList = new LinkedList<T>();

            // Deserialize ordered list
            for (int x = 0; x < info.GetInt32("orderedCount"); x++)
                Add((T)info.GetValue("orderedItem" + x, typeof(T)));
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="OrderedSet{T}"/>.</exception>
        public T this[int index]
        {
            get
            {
                return Visit(index).Value;
            }
            set
            {
                LinkedListNode<T> node = Visit(index);
                m_hashSet.Remove(node);
                node.Value = value;
                m_hashSet.Add(node);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="OrderedSet{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="OrderedSet{T}"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return m_hashSet.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}" /> object that is used to determine equality for the values in the set.
        /// </summary>
        /// <returns>
        /// The <see cref="IEqualityComparer{T}" /> object that is used to determine equality for the values in the set.
        /// </returns>
        public IEqualityComparer<T> Comparer
        {
            get
            {
                return m_equalityComparer;
            }
        }

        // Gets "false", indicating the hash-set is not read-only.
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added. 
        /// </summary>
        /// <returns>
        /// <c>true</c> if the element is added to the set; <c>false</c> if the element is already in the set.
        /// </returns>
        /// <param name="item">The element to add to the set.</param>
        public bool Add(T item)
        {
            return Insert(Count, item);
        }

        // Adds an element according to ICollection<T> interface specification
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="OrderedSet{T}"/> object.
        /// </summary>
        public void Clear()
        {
            m_hashSet.Clear();
            m_linkedList.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="OrderedSet{T}"/> object contains the specified element.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="OrderedSet{T}"/> object.</param>
        /// <returns>
        /// <c>true</c> if <see cref="OrderedSet{T}"/> object contains the specified element; otherwise,
        /// <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            if ((object)item == null)
                return false;

            return m_hashSet.Contains(new LinkedListNode<T>(item));
        }

        /// <summary>
        /// Copies the elements of the <see cref="OrderedSet{T}"/> to an <see cref="Array"/>.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from
        /// <see cref="OrderedSet{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the elements of the <see cref="OrderedSet{T}"/> to an <see cref="Array"/>, starting at the
        /// specified <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from
        /// <see cref="OrderedSet{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="arrayIndex"/> is greater than the the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, Count);
        }

        /// <summary>
        /// Copies the specified number of elements of the <see cref="OrderedSet{T}"/> to an <see cref="Array"/>,
        /// starting at the specified <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from
        /// <see cref="OrderedSet{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy to <paramref name="array"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0 -or - <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="arrayIndex"/> is greater than the the destination <paramref name="array"/> -or-
        /// <paramref name="count"/> is greater than the available space from the <paramref name="arrayIndex"/>
        /// to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            int index;

            if ((object)array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex must be less than the length of the array.");

            if (count <= array.Length - arrayIndex)
                throw new ArgumentException("count must be less than the available space in the array.");

            index = 0;

            foreach (T item in m_linkedList)
            {
                if (index >= count)
                    break;

                array[arrayIndex + index] = item;
                index++;
            }
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void ExceptWith(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            foreach (T item in other)
                Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="OrderedSet{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the <see cref="OrderedSet{T}"/>.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return m_linkedList.GetEnumerator();
        }

        // Gets a non-generic enumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_linkedList.GetEnumerator();
        }

        /// <summary>
        /// Implements the <see cref="ISerializable"/> interface and populates the <paramref name="info"/> object with 
        /// the data needed to serialize the serialize this <see cref="OrderedSet{T}"/> object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            int index = 0;

            info.AddValue("equalityComparer", m_equalityComparer, typeof(IEqualityComparer<T>));
            info.AddValue("orderedCount", Count);
            
            foreach (T item in m_linkedList)
            {
                info.AddValue("orderedItem" + index, item, typeof(T));
                index++;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="OrderedSet{T}"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="OrderedSet{T}"/>.</param>
        public int IndexOf(T item)
        {
            int index;

            if (Contains(item))
            {
                index = 0;

                foreach (T listItem in m_linkedList)
                {
                    if (m_equalityComparer.Equals(item, listItem))
                        return index;

                    index++;
                }
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="OrderedSet{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="OrderedSet{T}"/>.</param>
        /// <returns>True if the item was inserted; false otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="OrderedSet{T}"/>.</exception>
        public bool Insert(int index, T item)
        {
            LinkedListNode<T> node;
            LinkedListNode<T> newNode;

            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");

            if ((object)item == null)
                return false;

            newNode = new LinkedListNode<T>(item);

            if (!m_hashSet.Add(newNode))
                return false;

            if (index == Count)
            {
                m_linkedList.AddLast(newNode);
            }
            else
            {
                node = Visit(index);
                m_linkedList.AddBefore(node, newNode);
                m_lastVisitedNode = newNode;
            }

            return true;
        }

        /// <summary>
        /// Inserts an item to the <see cref="OrderedSet{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="OrderedSet{T}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="OrderedSet{T}"/>.</exception>
        void IList<T>.Insert(int index, T item)
        {
            Insert(index, item);
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            ExceptWith(this.Except(other));
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current set is a proper subset of <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            return m_hashSet.IsProperSubsetOf(other.Select(item => new LinkedListNode<T>(item)));
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current set is a proper superset of <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            int count = 0;

            if ((object)other == null)
                throw new ArgumentNullException("other");

            foreach (T item in other.Distinct())
            {
                if (!Contains(item))
                    return false;

                count++;
            }

            return Count != count;
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current set is a subset of <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            return m_hashSet.IsSubsetOf(other.Select(item => new LinkedListNode<T>(item)));
        }

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current set is a superset of <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            return other.All(Contains);
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current set and <paramref name="other"/> share at least one common element; otherwise,
        /// <c>false</c>.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            return other.Any(Contains);
        }

        /// <summary>
        /// Removes the the specified element from the <see cref="OrderedSet{T}"/> object.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> was successfully removed from the set; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if <paramref name="item"/> is not found in the 
        /// <see cref="OrderedSet{T}"/> object.
        /// </returns>
        /// <param name="item">The object to remove from the set.</param>
        public bool Remove(T item)
        {
            LinkedListNode<T> node;
            int index;

            if (!Contains(item))
                return false;

            // Find the node to be removed
            node = m_linkedList.First;
            index = 0;

            while (!m_equalityComparer.Equals(item, node.Value))
            {
                node = node.Next;
                index++;
            }

            if (index <= m_lastVisitedIndex)
            {
                // The last visited node's index is going to change,
                // so adjust the reference to the node that will take its place
                m_lastVisitedNode = m_lastVisitedNode.Next;

                // If the last visited node was at the end of the list before
                // the adjustment, set last visited index to its default of 0
                if ((object)m_lastVisitedNode == null)
                    m_lastVisitedIndex = 0;
            }

            m_hashSet.Remove(node);
            m_linkedList.Remove(node);

            return true;
        }

        /// <summary>
        /// Removes the <see cref="OrderedSet{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="OrderedSet{T}"/>.</exception>
        public void RemoveAt(int index)
        {
            LinkedListNode<T> node = Visit(index);

            // We just visited the node we are about to remove.
            // Since the next node will take its place,
            // set last visited node to the next node after it
            m_lastVisitedNode = m_lastVisitedNode.Next;

            // If the node to be removed is the last node in the list,
            // set last visited index back to its default of 0
            if ((object)m_lastVisitedNode == null)
                m_lastVisitedIndex = 0;

            m_hashSet.Remove(node);
            m_linkedList.Remove(node);
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate from the
        /// <see cref="OrderedSet{T}"/> object.
        /// </summary>
        /// <returns>
        /// The number of elements that were removed from the the <see cref="OrderedSet{T}"/> object.
        /// </returns>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the elements to remove.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public int RemoveWhere(Predicate<T> match)
        {
            LinkedListNode<T> node;
            LinkedListNode<T> nextNode;
            int itemsRemoved;

            if ((object)match == null)
                throw new ArgumentNullException("match");

            node = m_linkedList.First;
            itemsRemoved = 0;

            while ((object)node != null)
            {
                nextNode = node.Next;

                if (match(node.Value))
                {
                    m_hashSet.Remove(node);
                    m_linkedList.Remove(node);
                    itemsRemoved++;
                }

                node = nextNode;
            }

            m_lastVisitedIndex = 0;
            m_lastVisitedNode = null;

            return itemsRemoved;
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current set is equal to <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public bool SetEquals(IEnumerable<T> other)
        {
            int count = 0;

            foreach (T item in other.Distinct())
            {
                if (count > Count)
                    return false;

                if (!Contains(item))
                    return false;

                count++;
            }

            return Count == count;
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the
        /// current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            foreach (T item in other.Distinct())
            {
                if (Contains(item))
                    Remove(item);
                else
                    Add(item);
            }
        }

        /// <summary>
        /// Sets the capacity of this <see cref="OrderedSet{T}"/> object to the actual number of
        /// elements it contains, rounded up to a nearby, implementation-specific value.
        /// </summary>
        public void TrimExcess()
        {
            m_hashSet.TrimExcess();
        }

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in either the
        /// current set or the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public void UnionWith(IEnumerable<T> other)
        {
            if ((object)other == null)
                throw new ArgumentNullException("other");

            foreach (T item in other)
                Add(item);
        }

        private LinkedListNode<T> Visit(int index)
        {
            int firstDistance;
            int lastDistance;
            int visitedDistance;

            if (index < 0 || index >= m_linkedList.Count)
                throw new ArgumentOutOfRangeException("index");

            firstDistance = index;
            lastDistance = m_linkedList.Count - index;
            visitedDistance = Math.Abs(m_lastVisitedIndex - index);

            if (firstDistance <= lastDistance && firstDistance <= visitedDistance)
            {
                m_lastVisitedNode = m_linkedList.First;
                m_lastVisitedIndex = 0;
            }
            else if (lastDistance <= firstDistance && lastDistance <= visitedDistance)
            {
                m_lastVisitedNode = m_linkedList.Last;
                m_lastVisitedIndex = m_linkedList.Count;
            }

            while (m_lastVisitedIndex < index)
            {
                m_lastVisitedNode = m_lastVisitedNode.Next;
                m_lastVisitedIndex++;
            }

            while (m_lastVisitedIndex > index)
            {
                m_lastVisitedNode = m_lastVisitedNode.Previous;
                m_lastVisitedIndex--;
            }

            return m_lastVisitedNode;
        }

        #endregion
    }
}