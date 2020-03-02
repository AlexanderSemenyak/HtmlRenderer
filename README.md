**Hello !**

![nearly acid1](https://cloud.githubusercontent.com/assets/7447159/23646196/5c5c5096-0342-11e7-8d35-75b208206050.png)

_pic 1: HtmlRenderer, Gdi+, see [test file](https://github.com/LayoutFarm/HtmlRenderer/blob/master/Source/Test8_HtmlRenderer.Demo/Samples/0_acid1_dev/00.htm)_ 

**seems promising ?, NEARLY pass ACID1 test :)**

---

**Work In Progrss ... : Cross Platform HtmlRenderer**

The HtmlRenderer example!
---


![gles_html](https://user-images.githubusercontent.com/7447159/55290395-f93e8980-53fc-11e9-943a-a6ca6daf9aef.png)
_pic 2: HtmlRenderer on GLES2 surface, text are renderered with the Typography_


![gles_html](https://user-images.githubusercontent.com/7447159/55290398-12473a80-53fd-11e9-8135-62b707e52ad9.gif)
_pic 3: HtmlRenderer on GLES2 surface_


also, please note the text selection on the Html Surface.


(HtmlRender => https://github.com/LayoutFarm/HtmlRenderer,

Typography => https://github.com/LayoutFarm/Typography)


---

**How to build it**

see https://github.com/PaintLab/pxdev

---

 **Work In Progrss ... : Html-input**
 
 Html input elements are built with 'html-fragment (sub dom)'

 see more at https://github.com/LayoutFarm/HtmlRenderer/issues/18
 
![2019-03-31_21-55-37](https://user-images.githubusercontent.com/7447159/55290637-e6798400-53ff-11e9-8ca4-d3981e1da3d8.gif)

_pic3 : HtmlRenderer on GLES2 surface,developing Html-input, click to view full  size img_



---
The classic image.
![html_renderer_s01](https://cloud.githubusercontent.com/assets/7447159/24077194/3da7a684-0c78-11e7-8b83-98ebf77d5fdc.png)

 _pic 4: HtmlRenderer's Classic, Gdi+_
 


**MORE info / screen capture imgs** -> [see wiki](../../wiki/1.-Some-Screen-Captures)

**Build Note** -> [see wiki](../../wiki/3.-Build-The-Project)

-----
I forked this project from https://github.com/ArthurHub/HTML-Renderer (thank you so much)

I added some features

such as

1) dynamic html dom

2) decoupling, dependency analysis

3) optimizing the html,css parser. 
   see: HtmlKit v1.0(https://github.com/jstedfast/HtmlKit)    

4) add svg/canvas support (not complete)

5) abstract canvas backend (GDI+, OpenGL) also not complete for Linux (for the canvas backend, I used it from another project ->https://github.com/prepare/PixelFarm-dev)

6) Javascript (v8) binding (https://github.com/prepare/Espresso)

7) debug view

8) more layout support eg. inline-block,relative, absolute ,fixed, flex  etc 

9) added custom controls eg. text editer control, scrollbar, gridbox etc.

10) some events (eg. mouse /keyboard events)

.. BUT not complete :( 

feel free to fork/ comment/ suggest /pull request 


---
**Plan**

1) always permissive license (MIT,BSD, Apache2)

2) bind some features from Blink engine

3) add more html5/css3/js support

4) convert to C++ code with some transpiler tools 
   so users can build a final native code web browser

5) to make this runs on .NetCore

---
**Licenses**

The project is based on multiple open-sourced projects (listed below) **all using permissive licenses**.

A license for a whole project is [**MIT**](https://opensource.org/licenses/MIT).

but if you use some part of the code
please check each source file's header for the licensing info.


**Html Engine**

BSD, 2009, José Manuel Menéndez Poo, https://www.codeproject.com/Articles/32376/A-Professional-HTML-Renderer-You-Will-Use

BSD, 2013-2014, Arthur Teplitzki, from https://github.com/ArthurHub/HTML-Renderer

MIT, 2015, Jeffrey Stedfastm, from HtmlKit https://github.com/jstedfast/HtmlKit

**Javascript Engine**

MIT, 2013, Federico Di Gregorio, from https://github.com/Daniel15/vroomjs

MIT, 2015-present, WinterDev, from https://github.com/prepare/Espresso


**Geometry**

BSD, 2002-2005, Maxim Shemanarev, from http://www.antigrain.com , Anti-Grain Geometry - Version 2.4,

BSD, 2007-2014, Lars Brubaker, agg-sharp, from  https://github.com/MatterHackers/agg-sharp

ZLIB, 2015, burningmine, CurveUtils.

Boost, 2010-2014, Angus Johnson, Clipper.

BSD, 2009-2010, Poly2Tri Contributors, from https://github.com/PaintLab/poly2tri-cs

SGI, 2000, Eric Veach, Tesselate.


**Image Processing**

BSD, 2002-2005, Maxim Shemanarev, from http://www.antigrain.com , Anti-Grain Geometry - Version 2.4,

MIT, 2009-2015, Bill Reiss, Rene Schulte and WriteableBitmapEx Contributors, https://github.com/teichgraf/WriteableBitmapEx

MIT, 2008, dotPDN LLC, Rick Brewster, Chris Crosetto, Tom Jackson, Michael Kelsey, Brandon Ortiz, Craig Taylor, Chris Trevino, and Luke Walker., from OpenPDN v 3.36 (Paint.NET), https://github.com/rivy/OpenPDN


**Font**

MIT, 2016-present, WinterDev, Samuel Carlsson, Sam Hocevar and others, from https://github.com/LayoutFarm/Typography

Apache2, 2014-2016, Samuel Carlsson, from https://github.com/vidstige/NRasterizer

MIT, 2015, Michael Popoloski, from https://github.com/MikePopoloski/SharpFont

The FreeType Project LICENSE (3-clauses BSD style),2003-2016, David Turner, Robert Wilhelm, and Werner Lemberg and others, from https://www.freetype.org/

MIT, 2016, Viktor Chlumsky, from https://github.com/Chlumsky/msdfgen

**Platforms**

MIT, 2015-2015, Xamarin, Inc., from https://github.com/mono/SkiaSharp

MIT, 2006-2009,  Stefanos Apostolopoulos and other Open Tool Kit Contributors, from https://github.com/opentk/opentk

MIT, 2013, Antonie Blom, from  https://github.com/andykorth/Pencil.Gaming

MIT, 2004,2007, Novell Inc., for System.Drawing 


---

**Long Live Our Beloved C#**

WinterDev :)
