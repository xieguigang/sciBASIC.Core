﻿Namespace ComponentModel.Collection.Deque
    Public Module DequeTest
        Public Function GetReverseView(Of T)(ByVal d As Deque(Of T)) As IList(Of T)
            Return d.Reverse()
        End Function
    End Module
End Namespace