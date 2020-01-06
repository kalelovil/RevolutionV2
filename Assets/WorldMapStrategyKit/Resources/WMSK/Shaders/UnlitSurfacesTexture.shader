Shader "WMSK/Unlit Surface Texture" {
 
Properties {
    _Color ("Color", Color) = (1,1,1)
    _MainTex ("Texture", 2D) = "white"
}
 
SubShader {
    Tags {
        "Queue"="Geometry+1"
        "RenderType"="Opaque"
    }
    	
    //Lighting On // commented out to support LWRP using this same shader
    Blend SrcAlpha OneMinusSrcAlpha
    Material {
        Emission [_Color]
    }
   	ZWrite Off
   	Offset 1,1
    Pass {
    	SetTexture [_MainTex] {
            Combine Texture * Primary, Texture * Primary
        }
    }
}
 
}
