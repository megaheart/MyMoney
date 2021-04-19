# Sync Method

## I. Elements

### 1. Action Center

Which contain server-requesting and local-storing actions.

#### **Code structure**

```csharp
// Add and execute new action
InsertNewAction(Action action) 
// Load actions which isn't executed on starting and execute them
Load() 

// Find actions whose target items is the same and merge them
[private] ResolveConflictingActions() 
```

### 2. Actions

Which describe information about targets's changes and the way to execute them

#### **Code structure**

```csharp
abstract class Action

    // Execute it
    Execute() 

    // Check its failure's reasons and resolve
    ExecuteOnFailed() 

    TargetId { get; }

    // type code of target item
    TargetTypeId { get; } 

    // The lower the integer, the higher the priority
    // PriorityCode can be used to differentiate two different action types
    int PriorityCode { get; } 

    // Merge which other action, throw exception if they have different target types or different target codes
    Action Merge(Action action) 
```

#### **Actions Types**

1. Removing Action

```csharp
PriorityCode = 0
```

2. Adding Action

```csharp
PriorityCode = 1
```

3. Edit Action

```csharp
PriorityCode = 2
```

### 3. Synchronous Object

The items which can be stored on cloud.

#### **Code structure**

1. SyncId

```csharp
interface ISynchronousObject

// Null: if it is fail to synchronize item
// True if synchronize item successfully and the item exists on cloud
SyncId { get; }


```