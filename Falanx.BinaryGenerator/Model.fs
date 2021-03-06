namespace Falanx.BinaryGenerator
module Model =
    open Froto.Parser.Ast
    open System
    open ProviderImplementation.ProvidedTypes
    open Froto.Serialization.Encoding
    open Froto.Parser.ClassModel
    open Falanx.Ast.ProvidedTypesExtension
    
    type ProtobufType = string
    
    type TypeKind = 
        | Primitive
        | Class of scope: string * name : string
        | Enum of scope : string * fullName : string
        | Union of scope : string * name : string * fields : POneOfStatement list
    
    type TypeContext = 
        { Kind: TypeKind
          RuntimeType: Type
          ProtobufType: ProtobufType }
    
        member this.UnderlyingType =
            if this.RuntimeType.IsGenericType
            then this.RuntimeType.GenericTypeArguments.[0]
            else this.RuntimeType
            
    type PropertyDescriptor = 
        { Position: FieldNum
          Rule: ProtoFieldRule 
          Type: TypeContext
          ProvidedProperty: ProvidedProperty
          ProvidedField: ProvidedField option }
          
    type OneOfDescriptor =
        { Properties:  Map<int, PropertyDescriptor>
          Type: TypeContext
          OneOfType: ProvidedUnion
          OneOfMembers: POneOfStatement list
          CaseProperty: ProvidedProperty
          CaseField: ProvidedField }
          
    type MapDescriptor = 
        { KeyType: TypeContext
          ValueType: TypeContext
          Position: FieldNum
          ProvidedProperty: ProvidedProperty
          ProvidedField: ProvidedField }
          
    type TypeDescriptor = 
        { Type: ProvidedTypeDefinition
          Properties: PropertyDescriptor list
          OneOfGroups: OneOfDescriptor list
          Maps: MapDescriptor list }
          
    type GenerationType =
        //ProvidedRecords will have a parent it they are defined inside another message, top level ones will not
        | ProvidedRecord of ProvidedRecord * parent: ProvidedRecord option
        //ProvidedUnions will always have a parent as they are specific to each message
        | ProvidedUnion of ProvidedUnion * parent: ProvidedRecord option
        //|ProvidedType of ProvidedType