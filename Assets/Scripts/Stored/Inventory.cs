using System;
using System.Collections.Generic;
using System.Linq;

namespace Stored
{
    public class Inventory
    {
        private readonly Artefact[] items;
        private int lastEmptySlot = 0;
        private int itemCount;

        /// <summary>
        /// Displays How many slots there are in a given inventory, how much space there is.
        /// </summary>
        public int Size => items.Length;
        public int ItemCount => itemCount;
        public int SpaceLeftCount => Size - itemCount;
        /// <summary>
        /// Get all items that are in the inventory.
        /// </summary>
        public IEnumerable<Artefact> Items => items.Where(i => i != null);

        public Inventory(int size)
        {
            items = new Artefact[size];
        }

        /// <summary>
        /// Add an item at the best empty slot.
        /// </summary>
        /// <returns>True if item is successfully added. Otherwise there may be no space.</returns>
        public bool AddItem(Artefact item)
        {
            if (!HasSpace()) return false;
            items[lastEmptySlot] = item;
            lastEmptySlot = GetNextEmptySlot(lastEmptySlot);
            itemCount++;
            return true;
        }

        /// <summary>
        /// Add an item at a specific slot.
        /// </summary>
        /// <returns>True if item is successfully added. Otherwise there may be no space.</returns>
        /// <exception cref="IndexOutOfRangeException">If the slot number is not valid.</exception>
        public bool AddItem(Artefact item, int slot)
        {
            AssertSlot(slot);
            if (IsOccupiedAt(slot)) return false;
            items[slot] = item;
            
            if (lastEmptySlot == slot)
                lastEmptySlot = GetNextEmptySlot(slot);

            itemCount++;
            return true;
        }

        /// <summary>
        /// Move an existing item to another slot in the inventory.
        /// </summary>
        /// <returns>True if item is successfully moved. Otherwise slots may be occupied.</returns>
        /// <exception cref="IndexOutOfRangeException">If the slot number is not valid.</exception>
        public bool MoveItem(int fromSlot, int toSlot)
        {
            AssertSlot(fromSlot);
            AssertSlot(toSlot);
            if (!IsOccupiedAt(fromSlot) || IsOccupiedAt(toSlot)) return false;
            items[toSlot] = items[fromSlot];
            items[fromSlot] = null;

            if (lastEmptySlot == toSlot)
                lastEmptySlot = GetEmptySlot();
            
            return true;
        }

        /// <summary>
        /// Completely remove an item from a given slot.
        /// </summary>
        /// <returns>True if operation is successful. Otherwise there may be no item at that slot.</returns>
        /// <exception cref="IndexOutOfRangeException">If the slot number is not valid.</exception>
        public bool RemoveItem(int slot)
        {
            AssertSlot(slot);
            if (!IsOccupiedAt(slot)) return false;
            items[slot] = null;

            if (lastEmptySlot == -1)
                lastEmptySlot = slot;
            else if (slot < lastEmptySlot)
                lastEmptySlot = slot;

            itemCount--;
            return true;
        }

        public bool RemoveItem(Artefact item)
        {
            int? slot = FindItemSlot(item);
            return slot.HasValue && RemoveItem(slot.Value);
        }

        /// <summary>
        /// Get an item at some slot, if nothing is there then it will return null.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">If the slot number is not valid.</exception>
        public Artefact GetItemAtOrNull(int slot)
        {
            AssertSlot(slot);
            return items[slot];
        }

        /// <summary>
        /// Get the item slot for a certain item. If the item does not exist in this inventory, return null.
        /// </summary>
        public int? FindItemSlot(Artefact item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == item)
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Check if a certain item exist in this inventory.
        /// </summary>
        public bool Contains(Artefact item) => Items.Contains(item);

        /// <summary>
        /// Can we add more items into the inventory?
        /// </summary>
        public bool HasSpace() => lastEmptySlot != -1;

        /// <summary>
        /// Is there an item currently at that slot?
        /// </summary>
        public bool IsOccupiedAt(int slot) => items[slot] != null;

        /// <summary>
        /// Check if the slot exist, can be accessed in the inventory. It should be within the inventory size.
        /// </summary>
        public bool IsValidSlot(int slot) => slot >= 0 && slot < Size;
        
        /// <summary>
        /// Try to get the next available empty slot after some slot.
        /// More efficient than <c>GetEmptySlot()</c>.
        /// </summary>
        private int GetNextEmptySlot(int afterSlot)
        {
            int currentSlot = afterSlot;

            do
            {
                currentSlot++;

                if (currentSlot >= Size)
                    return -1;
            }
            while (IsOccupiedAt(currentSlot));

            return currentSlot;
        }
        
        /// <summary>
        /// This tries to get a free empty slot from everything with no assumptions though brute force.
        /// </summary>
        private int GetEmptySlot()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (!IsOccupiedAt(i))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Throw an error if such slot is not valid.
        /// </summary>
        private void AssertSlot(int slot)
        {
            if (!IsValidSlot(slot))
                throw new IndexOutOfRangeException($"Slot {slot} is not a valid slot number");
        }

        /// <summary>
        /// Display a readable, DEBUG friendly display of the inventory.
        /// </summary>
        public string ReadFlat()
        {
            var elements = items.Select(i => i == null ? "EMPTY" : i.DisplayName);
            return "[" + string.Join(", ", elements) + " ]";
        }
    }
}