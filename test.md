### [234\. Palindrome Linked List](https://leetcode.com/problems/palindrome-linked-list/description/)

Difficulty: **Easy**



Given a singly linked list, determine if it is a palindrome.

**Example 1:**

```
**Input:** 1->2
**Output:** false```

**Example 2:**

```
**Input:** 1->2->2->1
**Output:** true```

**Follow up:**  
Could you do it in O(n) time and O(1) space?



#### Solution
```python
# Definition for singly-linked list.
# class ListNode:
#     def __init__(self, x):
#         self.val = x
#         self.next = None
​
class Solution:
    def isPalindrome(self, head):
        """
        :type head: ListNode
        :rtype: bool
        """
        temp_head = head
        temp_stack = list()
        while temp_head is not None:
            print(temp_head.val)
            temp_stack.append(temp_head.val)
            temp_head = temp_head.next
        while head is not None:
            if head.val != temp_stack.pop():
                return False
            head = head.next
​
        return True     
```
