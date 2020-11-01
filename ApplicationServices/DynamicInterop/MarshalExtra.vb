﻿Imports System
Imports System.Runtime.InteropServices

Namespace ApplicationServices.DynamicInterop
    ''' <summary> 
    ''' Extra methods on top of System.Runtime.InteropServices.Marshal for allocating unmanaged memory, copying unmanaged
    ''' memory blocks, and converting managed to unmanaged types</summary>
    Friend Class MarshalExtra
        ''' <summary> Allocates memory from the unmanaged memory of the process for a given type</summary>
        '''
        ''' <typeparamname="T"> A type that has an equivalent in the unmanaged world e.g. int or a struct Point </typeparam>
        '''
        ''' <returns> A pointer to the newly allocated memory. This memory must be released using the FreeHGlobal(IntPtr) method, or related.</returns>
        Public Shared Function AllocHGlobal(Of T)() As IntPtr
            Dim iSize = Marshal.SizeOf(GetType(T))
            Return Marshal.AllocHGlobal(iSize)
        End Function

        ''' <summary> Marshals data from an unmanaged block of memory to a newly allocated managed object of the type specified by a generic type parameter. 
        '''           Note it is almost superseded in .NET Framework 4.5.1 and later versions; consider your needs</summary>
        '''
        ''' <exceptioncref="ArgumentException"> Thrown when one or more arguments have unsupported or
        '''                                      illegal values.</exception>
        '''
        ''' <typeparamname="T"> The type of the object to which the data is to be copied. This must be a structure.</typeparam>
        ''' <paramname="ptr">   A pointer to an unmanaged block of memory.</param>
        ''' <paramname="cleanup"> (Optional) If true, free the native memory block pointed to by ptr. This feature is handy in generated marshalling code.</param>
        '''
        ''' <returns> A managed object that contains the data that the ptr parameter points to</returns>
        Public Shared Function PtrToStructure(Of T As Structure)(ByVal ptr As IntPtr, ByVal Optional cleanup As Boolean = False) As T
            If ptr = IntPtr.Zero Then Throw New ArgumentException("pointer must not be IntPtr.Zero")
            Dim result As T = Marshal.PtrToStructure(ptr, GetType(T))
            If cleanup Then Marshal.FreeHGlobal(ptr)
            Return result
        End Function

        ''' <summary>Marshals data from a managed object of a specified type to an unmanaged block of memory.
        '''           Note it is almost superseded in .NET Framework 4.5.1 and later versions; consider your needs</summary>
        '''
        ''' <typeparamname="T"> The type of the managed object.</typeparam>
        ''' <paramname="structure"> A managed object that holds the data to be marshaled. The object must be a structure.</param>
        '''
        ''' <returns> A pointer to a newly allocated unmanaged block of memory.</returns>
        Public Shared Function StructureToPtr(Of T As Structure)(ByVal [structure] As T) As IntPtr
            Dim ptr = IntPtr.Zero
            Dim localStruct = [structure]
            Dim iSize = Marshal.SizeOf(GetType(T))
            ptr = Marshal.AllocHGlobal(iSize)
            Marshal.StructureToPtr(localStruct, ptr, False)
            Return ptr
        End Function

        ''' <summary> Frees all substructures of a specified type that the specified unmanaged memory block points to.</summary>
        '''
        ''' <typeparamname="T"> The type of the managed object.</typeparam>
        ''' <paramname="ptr">           A pointer to an unmanaged block of memory.</param>
        ''' <paramname="managedObject"> [in,out] The managed object.</param>
        ''' <paramname="copy">          (Optional) True to copy.</param>
        Public Shared Sub FreeNativeStruct(Of T As Structure)(ByVal ptr As IntPtr, ByRef managedObject As T, ByVal Optional copy As Boolean = False)
            If ptr = IntPtr.Zero Then Return '?

            If copy Then
                managedObject = CType(Marshal.PtrToStructure(ptr, GetType(T)), T)
                ' Marshal.PtrToStructure<T>(ptr, managedObject);
            End If

            Marshal.FreeHGlobal(ptr)
        End Sub

        Public Shared Function ArrayOfStructureToPtr(Of T As Structure)(ByVal managedObjects As T()) As IntPtr
            Dim structSize As Integer = Marshal.SizeOf(Of T)()
            Dim result = Marshal.AllocHGlobal(managedObjects.Length * structSize)

            For i = 0 To managedObjects.Length - 1
                Dim offset = i * structSize
                Marshal.StructureToPtr(managedObjects(i), IntPtr.Add(result, offset), False)
            Next

            Return result
        End Function

        Public Shared Sub FreeNativeArrayOfStruct(Of T As Structure)(ByVal ptr As IntPtr, ByRef managedObjects As T(), ByVal Optional copy As Boolean = False)
            If ptr = IntPtr.Zero Then Return
            Marshal.FreeHGlobal(ptr)
        End Sub
    End Class
End Namespace
