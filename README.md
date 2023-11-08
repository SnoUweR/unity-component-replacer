## Component Replacer

This utility allows you to replace the component with another component of the same class hierarchy.
All properties that exist in both components are copied to the new component.
Therefore we can easily replace a base component with any derivative, or a derived component to another
derived component (or even to the base one) without having to connect all properties links again.

### Installation

*Requires Unity 2018.4+*

#### Install via UPM (using Git URL)

1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the "dependencies": { line
    - ```json title="Packages/manifest.json"
      "ru.snouwer.component-replacer": "https://github.com/SnoUweR/unity-component-replacer.git",
      ```
3. UPM should now install the package.

### Video Examples

#### Default way without Component Replacer
https://user-images.githubusercontent.com/576036/134950777-f3407f90-695f-4d4b-ba8a-665862f72882.mp4

#### New way with Component Replacer
https://user-images.githubusercontent.com/576036/134950752-b770e20d-d0ac-40dc-80a2-8d05229c6c28.mp4
