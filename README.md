# FLVER Editor 2.0

FLVER Editor 2.0 is a complete re-imagination of the original FLVER Editor model editing tool, designed to modify the FLVER model file format for the mainline FromSoftware series of games.

Patch Notes - Version 1.73:

-New and easy to use installer allows users to get up and running with the tool with minimal to no fuss

-Comprehensive filters list for all of the available model container formats located in the bottom right of the Open FLVER File dialog window

-Completely re-designed user interface allows for more efficient user interaction and heightened productivity

-Loading time for the viewer window has been greatly decreased, allowing artists to better focus on their work

-Viewer background has been changed from a solid blue to a gray/dark gray gradient, providing increased contrast and comfort for the eyes

-The Alt+Left Click shortcut can now be used to move around within the viewer, which is especially useful for those without a middle-mouse button

-The X and Z axis lines rendered within the viewer window now provide accurate symmetry, and all axis lines have been extended to a length of 1000

-A feature for changing the thickness level of dummy points, ranging from 1-15, has now been implemented, and is available in the Preferences menu

-The filename of the currently opened FLVER file is now displayed in the title bar of the main editor window

-The program's version number is displayed in the top right corner of the main editor window, along with copyright information

-Controls and functionality for bones, materials, and mesh are now placed within individual tabs in the main editor window

-Columns for modifying the minimum and maximum bounding box values of bones have now been added in the Bones table

-The Index and Name columns within every data table in the main editor window are frozen by default to provide accessibility when scrolling horizontally

-The Position, Scale, and Rotation values are now linked up properly, as opposed to the old tool where changing these values would not modify them in the file

-A brand new feature named Material Presets has been introduced in this version, which allows users to change and swap materials with ease

-Material deletion is now supported, allowing users to eliminate redundant materials, along with a Delete All button in the Materials table

-Materials and textures are now represented as individual data tables in the Materials tab, along with a line splitter to more easily control visibility

-The tool now automatically fixes every material's Unk18 field, which now fixes material issues with cutscene playback in Elden Ring and potentially other games

-Material textures can now be viewed and modified by clicking on the Edit button which corresponds with the desired material in the Materials table

-The DDS file format is now enforced when browsing for a new texture file to replace a pre-existing material's texture

-The tool now automatically adjusts the file extension displayed in the path entry for a texture to .TIF when browsing for a new texture file

-A new feature has been implemented for applying texture entries from any .MATBIN file to the internal texture entries for a material

-The tool now supports viewer highlighting for materials, allowing artists to more easily identify meshes that have a particular material applied to them

-Features for selecting all of the mesh and dummy points in the currently opened FLVER file have been added in the form of buttons towards the top of the Mesh tab

-Features for solving and adjusting the bone and mesh bounding boxes of a FLVER file have been added in the form of buttons at the top of the Mesh tab

-A new column has been added in the Mesh data table for displaying the first value of the bone indices list for a given mesh in the currently opened FLVER file

-The user can now create new dummy points using the New Dummy button above the Dummies table in the Mesh tab, eliminating the need to outsource the model to Blender

-Similar to material presets, dummy presets allow the user to take a snapshot of all of the dummy points in the model, and re-use them later

-Reference IDs for dummy points are now displayed in a new column in the Dummies data table, allowing users to cross-reference dummy points from other programs

-Two new columns have been added for changing the Attach Bone Index and Parent Bone Index properties of a dummy point, allowing for easier interaction with bones

-Dummy points can now be duplicated and re-used with the same exact properties, allowing users to easily replicate pre-existing effects without compromising control

-The Modifiers panel has been added to the Mesh tab, containing all controls used for modifying mesh and dummy points in a single, unified location

-All mesh transform operations now occur relative to the center point, or centroid, of a given mesh, as opposed to the world's own center point

-Uniform scaling has now been implemented for meshes, allowing users to make big their dreams and crush their worries, all while retaining proportion

-Axis labels have been added in the Modifiers panel to provide easier accessibility when transforming mesh and dummy points when the viewer orientation is changed

-It's real, and there's no time to waste! Real-time mesh and dummy point transformations allow artists to perfect their creative visions with the utmost precision

-Number box arrows have now been added for finer control when modifying mesh and dummy points in difficult to get right situations

-A new system has been implemented for scaling mesh and dummy points, allowing artists to feel closer to home after using other 3D software such as Blender

-A new toolset has been implemented for mirroring mesh and its respective dummy points across any world axis, reducing the need for third-party software

-The center to world feature saves artists from deadly catastrophes with extremely off-centered meshes by re-locating them to the world's center point: phew!

-Mesh and dummy deletion is now supported, meaning artists don't have to deal with their leftovers, and can instead focus on creating with minimal clutter

-A whole bunch of other stuff, go find out for yourself!
