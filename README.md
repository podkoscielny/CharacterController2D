# CharacterController2D
Physics based controller for Unity 2D platformer games.    

## Navigation
* [Features](#features)
* [Setup Guide](#setup-guide)
* [Methods](#methods)
* [Events](#events)

## Features
- _**Physics** based movement_
- _**Crouch** support_
- _Decide whether to control character in **air** or not_
- _**Double** jump support_

## Setup Guide

### Installation
  Download the zip file and place CharacterController2D.cs script in your project.
  
  ### Usage
  
  #### Add Rigidbody2D and 2D collider of your choice to character GameObject.
  <img src="https://user-images.githubusercontent.com/49119130/165386928-044a2b0f-ac8b-4c6d-97c9-7920bf1f15c7.png" width="450" />
  
  #### Attach CharacterController2D.
  <img src="https://user-images.githubusercontent.com/49119130/165387394-545b4904-8748-4abb-bf91-61838220ab02.png" width="450" />
  
  #### Add empty GameObjects on top and bottom of your character for ground and ceiling check.
  <img src="https://user-images.githubusercontent.com/49119130/165387894-874c9295-9bd2-4837-96f2-d4301548965f.png" width="450" />
  <img src="https://user-images.githubusercontent.com/49119130/165387941-5e6d1050-dc91-4985-9dc0-1d719eb80b3e.png" width="450" />
  
  #### Drag Rigidbody2D component to CharacterController2D
  <img src="https://user-images.githubusercontent.com/49119130/165388414-fb3f91de-554f-4715-a96e-504ae9ce41ca.png" width="450" />
  
  #### Drag ground and ceiling check GameObjects to CharacterController2D in their respective fields in Collisions section.
  <img src="https://user-images.githubusercontent.com/49119130/165388780-8d60451b-cac4-495a-aa2d-796d7a520be0.png" width="450" />
  
  #### Choose layer masks that will act as a walkable surface for the character. 
  <img src="https://user-images.githubusercontent.com/49119130/165388975-b9fd4fc7-3eba-46db-88fd-4f91aa01ad71.png" width="450" />
  
  #### Adjust ground and ceiling check sizes so that it fits character and Controller can check collisions properly. You can see their sizes in Scene view (make sure Gizmos are enabled).
  <img src="https://user-images.githubusercontent.com/49119130/165389389-70806e4c-1d06-4b66-b825-391653aff520.png" width="450" />
  <img src="https://user-images.githubusercontent.com/49119130/165389412-6866589a-eaf2-4551-a5f0-aeb5d208d902.png" width="400" />
  
  #### Finally, set options of your choice in Movement section
  <img src="https://user-images.githubusercontent.com/49119130/165389695-23b41f8f-9578-4582-b44a-d1013080fd28.png" width="500" />
  
  ##### Crouch speed (if you don't plan to use crouch in your game this setting doesn't matter)
  
  ##### Jump force
  
  ##### Air control - choose whether to move character when it is not grounded
  
  ##### Can Double Jump
  
  ##### Can Jump On Fall - choose whether to enable jump when character loses the ground and starts to fall

## Methods

Make sure to include `using AoOkami.CharacterController;` in your movement script.

### Move
**Move(float moveAmount)** function takes float value as an argument, which stands for amount it should move the character by in horizontal direction.
Negative value moves the object to the left, while positive to the right.
As CharacterController2D is physics based controller you should invoke this function in FixedUpdate.
<br />

```csharp
using AoOkami.CharacterController;

[SerializeField] CharacterController2D characterController;

private void FixedUpdate() => characterController.Move(_horizontalMovement);
```

### Jump
**Jump()** function adds impulse force to the rigidbody. Force amount is specified in Jump Force field.
Since ForceMode is Impulse there is no need to invoke this method in FixedUpdate.

```csharp
using AoOkami.CharacterController;

[SerializeField] CharacterController2D characterController;

private void Update()
{
    if (Input.GetKeyDown(KeyCode.Space)) characterController.Jump();
}
```

### Crouch
**Crouch(bool isCrouching)** method takes isCrouching bool argument. When **true** is passed, character's speed is slowed down by factor specified in Crouch Speed field. 
While crouching, jumping is not possible. 
When **false** is passed, character's speed returns to normal, unless ceiling is low enough, so that character cannnot stand up. 
In that case crouching will be released, when there is no ceiling above.

```csharp
using AoOkami.CharacterController;

[SerializeField] CharacterController2D characterController;

private void Update()
{
    if (Input.GetKeyDown(KeyCode.C)) characterController.Crouch(true);
     
    if (Input.GetKeyUp(KeyCode.C)) characterController.Crouch(false);
}
```

## Events
Here is the list of C# events available, that will help you trigger character animations or custom methods when needed.

### OnLanded
Invokes when character wasn't grounded and makes contact with an object, which Layer is same as Ground Mask.   
Example usage:

```csharp
using AoOkami.CharacterController;

[SerializeField] Animator characterAnimator;

private void OnEnable() => CharacterController2D.OnLanded += SetLandingAnimation;

private void OnDisable() => CharacterController2D.OnLanded -= SetLandingAnimation;

private void SetLandingAnimation() => characterAnimator.SetBool("IsGrounded", true);
```

### OnFalling
Invokes when there is no ground beneath character.
Example usage:

```csharp
using AoOkami.CharacterController;

[SerializeField] Animator characterAnimator;

private void OnEnable() => CharacterController2D.OnFalling += SetFallingAnimation;

private void OnDisable() => CharacterController2D.OnFalling -= SetFallingAnimation;

private void SetFallingAnimation() => characterAnimator.SetBool("IsGrounded", false);
```

### OnJump
Example usage:

```csharp
using AoOkami.CharacterController;

[SerializeField] Animator characterAnimator;

private void OnEnable() => CharacterController2D.OnJump += SetJumpAnimation;

private void OnDisable() => CharacterController2D.OnJump -= SetJumpAnimation;

private void SetJumpAnimation() => characterAnimator.SetTrigger("Jump");
```

### OnDoubleJump
Example usage:

```csharp
using AoOkami.CharacterController;

[SerializeField] Animator characterAnimator;

private void OnEnable() => CharacterController2D.OnDoubleJump += SetDoubleJumpAnimation;

private void OnDisable() => CharacterController2D.OnDoubleJump -= SetDoubleJumpAnimation;

private void SetDoubleJumpAnimation() => characterAnimator.SetTrigger("DoubleJump");
```

### \<bool> OnCrouch
OnCrouch can be subscribed to by method that takes bool argument. Thanks to that you can easily determine when character started or stopped crouching. 

```csharp
using AoOkami.CharacterController;

[SerializeField] Animator characterAnimator;

private void OnEnable() => CharacterController2D.OnCrouch += SetCrouchAnimation;

private void OnDisable() => CharacterController2D.OnCrouch -= SetCrouchAnimation;

private void SetCrouchAnimation(bool isCrouching) => characterAnimator.SetBool("IsCrouching", isCrouching);
```
