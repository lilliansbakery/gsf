'*******************************************************************************************************
'  AnalogDefinition.vb - IEEE C37.118 Analog definition
'  Copyright � 2008 - TVA, all rights reserved - Gbtc
'
'  Build Environment: VB.NET, Visual Studio 2008
'  Primary Developer: J. Ritchie Carroll, Operations Data Architecture [TVA]
'      Office: COO - TRNS/PWR ELEC SYS O, CHATTANOOGA, TN - MR 2W-C
'       Phone: 423/751-2827
'       Email: jrcarrol@tva.gov
'
'  Code Modification History:
'  -----------------------------------------------------------------------------------------------------
'  11/12/2004 - J. Ritchie Carroll
'       Initial version of source generated
'
'*******************************************************************************************************

Imports System.Runtime.Serialization
Imports System.Text

Namespace IeeeC37_118

    <CLSCompliant(False), Serializable()> _
    Public Class AnalogDefinition

        Inherits AnalogDefinitionBase

        Private m_type As AnalogType

        Protected Sub New()
        End Sub

        Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)

            MyBase.New(info, context)

            ' Deserialize analog definition
            m_type = info.GetValue("type", GetType(AnalogType))

        End Sub

        Public Sub New(ByVal parent As ConfigurationCell)

            MyBase.New(parent)

        End Sub

        Public Sub New(ByVal parent As ConfigurationCell, ByVal binaryImage As Byte(), ByVal startIndex As Int32)

            MyBase.New(parent, binaryImage, startIndex)

        End Sub

        Public Sub New(ByVal parent As ConfigurationCell, ByVal index As Int32, ByVal label As String, ByVal scale As Int32, ByVal offset As Single)

            MyBase.New(parent, index, label, 1, 0)

        End Sub

        Public Sub New(ByVal parent As ConfigurationCell, ByVal analogDefinition As IAnalogDefinition)

            MyBase.New(parent, analogDefinition)

        End Sub

        Friend Shared Function CreateNewAnalogDefintion(ByVal parent As IConfigurationCell, ByVal binaryImage As Byte(), ByVal startIndex As Int32) As IAnalogDefinition

            Return New AnalogDefinition(parent, binaryImage, startIndex)

        End Function

        Public Overrides ReadOnly Property DerivedType() As System.Type
            Get
                Return Me.GetType
            End Get
        End Property

        Public Shadows ReadOnly Property Parent() As ConfigurationCell
            Get
                Return MyBase.Parent
            End Get
        End Property

        Public Property [Type]() As AnalogType
            Get
                Return m_type
            End Get
            Set(ByVal value As AnalogType)
                m_type = value
            End Set
        End Property

        Friend Shared ReadOnly Property ConversionFactorLength() As Int32
            Get
                Return 4
            End Get
        End Property

        Friend ReadOnly Property ConversionFactorImage() As Byte()
            Get
                Dim buffer As Byte() = CreateArray(Of Byte)(ConversionFactorLength)

                buffer(0) = m_type

                EndianOrder.BigEndian.CopyBuffer(BitConverter.GetBytes(ScalingFactor), 0, buffer, 1, 3)

                Return buffer
            End Get
        End Property

        Friend Sub ParseConversionFactor(ByVal binaryImage As Byte(), ByVal startIndex As Int32)

            Dim buffer As Byte() = CreateArray(Of Byte)(4)

            ' Get analog type from first byte
            m_type = binaryImage(startIndex)

            ' Last three bytes represent scaling factor
            EndianOrder.BigEndian.CopyBuffer(binaryImage, startIndex + 1, buffer, 0, 3)
            ScalingFactor = BitConverter.ToInt32(buffer, 0)

        End Sub

        Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

            MyBase.GetObjectData(info, context)

            ' Serialize analog definition
            info.AddValue("type", m_type, GetType(AnalogType))

        End Sub

        Public Overrides ReadOnly Property Attributes() As Dictionary(Of String, String)
            Get
                Dim baseAttributes As Dictionary(Of String, String) = MyBase.Attributes

                baseAttributes.Add("Analog Type", [Type] & ": " & [Enum].GetName(GetType(AnalogType), [Type]))

                Return baseAttributes
            End Get
        End Property

    End Class

End Namespace