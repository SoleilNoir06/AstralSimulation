#version 330

in vec2 fragTexCoord;

uniform sampler2D texture0;
uniform sampler2D oTexture0;

uniform vec4 sunCol;
uniform vec2 sourcePos;
uniform vec2 iResolution;
uniform float camDist;
uniform float time;

out vec4 finalColor;

// Custom functions
float rnd(vec2 p)
{
    float f = fract(sin(dot(p, vec2(12.1234, 72.8392) )*45123.2));
    return f;   
}

float rnd(float w)
{
    float f = fract(sin(w)*1000.);
    return f;   
}

float regShape(vec2 p, int N)
{
    float f;
    
    float a=atan(p.x,p.y)+.2;
    float b=6.28319/float(N);
    f=smoothstep(.5,.51, cos(floor(.5+a/b)*b-a)*length(p.xy));
    
    return f;
}

vec3 circle(vec2 p, float size, float decay, vec3 color,vec3 color2, float dist, vec2 mouse)
{
    //l is used for making rings.I get the length and pass it through a sinwave
    //but I also use a pow function. pow function + sin function , from 0 and up, = a pulse, at least
    //if you return the max of that and 0.0.
    
    float l = length(p + mouse*(dist*4.))+size/2.;
    
    //l2 is used in the rings as well...somehow...
    float l2 = length(p + mouse*(dist*4.))+size/3.;
    
    ///these are circles, big, rings, and  tiny respectively
    float c = max(00.01-pow(length(p + mouse*dist), size*1.4), 0.0)*50.;
    float c1 = max(0.001-pow(l-0.3, 1./40.)+sin(l*30.), 0.0)*3.;
    float c2 =  max(0.04/pow(length(p-mouse*dist/2. + 0.09)*1., 1.), 0.0)/20.;
    float s = max(00.01-pow(regShape(p*5. + mouse*dist*5. + 0.9, 6) , 1.), 0.0)*8.;
    
    color = 0.5+0.5*sin(color);
    color = cos(vec3(0.44, .24, .2)*8. + dist*4.)*0.5+.5;
    vec3 f = c*color ;
    f += c1*color;
    
    f += c2*color;  
    f +=  s*color;
    return f-0.01;
}

float sun(vec2 p, vec2 mouse)
{
 float f;
    
    vec2 sunp = p+mouse;
    float sun = 1.0-length(sunp)*8.;
    return f;
}

void main()
{
    // Get planet occlusion
    float occlusion = texture(oTexture0, fragTexCoord).r;

    vec2 resol = normalize(iResolution.xy);

    vec2 uv = fragTexCoord;
    //uv=uv*2.-1.0;
    uv.x*=resol.x/resol.y;
    
    vec2 mm = sourcePos.xy / -vec2(-iResolution.x, iResolution.y);
    mm.x *= resol.x/resol.y;
    mm.y += 0.98;
    
    vec3 circColor = vec3(0.9, 0.2, 0.1);
    vec3 circColor2 = vec3(0.3, 0.1, 0.5);
    
    //now to make the sky not black
    vec3 color = mix(vec3(0.0, 0.0, 0.00)/1.0, vec3(0.0, 0.0, 0.0), uv.y)*3.-0.52*sin(0.2)*0.000001+0.000001;
    
    //this calls the function which adds three circle types every time through the loop based on parameters I
    //got by trying things out. rnd i*2000. and rnd i*20 are just to help randomize things more
    for(float i=0.;i<10.;i++){
        color += circle(uv, pow(rnd(i*2000.)*1.0, 2.)+1.41, 0.0, circColor+i , circColor2+i, rnd(i*20.)*3.+0.2-.5, mm);
    }
    //get angle and length of the sun (uv - mouse)
    float a = atan(uv.y-mm.y, uv.x-mm.x);
    float l = max(1.0-length(uv-mm)-0.84, 0.0);
    
    float bright = 0.134;//+0.1/1/3.;//add brightness based on how the sun moves so that it is brightest
    //when it is lined up with the center
    
    //add the sun with the frill things
    color += max(0.1/pow(length(uv-mm)*5., 5.), 0.0)*abs(sin(a*5.+cos(a*9.)))/15;
    color += max(0.1/pow(length(uv-mm)*10., 1./20.), .0)+abs(sin(a*3.+cos(a*9.)))/8.*(abs(sin(a*9.)))/20.;

    //add another sun in the middle (to make it brighter)  with the20color I want, and bright as the numerator.
    color += (max(bright/pow(length(uv-mm)*4., 1./2.), 0.0)*4.)*sunCol.rgb*2.5;
    
    //multiply by the exponetial e^x ? of 1.0-length which kind of masks the brightness more so that
    //there is a sharper roll of of the light decay from the sun. 
    color*= exp(1.0-length(uv-mm))/5.;
    finalColor = vec4(color,0.5)*camDist;

    // Mix between render and calculations
    vec4 renderColor = texture(texture0, fragTexCoord);
    finalColor = mix(finalColor, renderColor, 0.5);
    finalColor = mix(finalColor, renderColor, occlusion);
}