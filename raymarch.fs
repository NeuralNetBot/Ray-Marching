#version 330 core

uniform vec2 screensize;

uniform float Power;

uniform float Bailout;

uniform vec3 Position;
uniform vec3 LookAt;

out vec4 color;

struct Ray
{
    vec3 pos;
    vec3 dir;
};

Ray RayFromCamera(vec2 screenPos, vec3 cameraPos, vec3 lookAt, float zoom)
{
    vec3 f = normalize(lookAt - cameraPos);
    vec3 r = cross(vec3(0.0, 1.1, 0.0), f);
    vec3 u = cross(f, r);
    
    vec3 c = cameraPos + f * zoom;
    vec3 i = c + screenPos.x * r + screenPos.y * u;
    vec3 dir = i - cameraPos;
    return Ray(cameraPos, dir);
}

float epsilon = 0.0001;

vec2 distToScene(vec3 p)
{

	vec3 z = p;
	float dr = 1.0;
	float r = 0.0;
    
    int i;
    
	for (i = 0; i < 15 ; i++)
    {
		r = length(z);
		if (r>Bailout) break;
		
		float theta = acos(z.z/r);
		float phi = atan(z.y,z.x);
		dr =  pow( r, Power-1.0)*Power*dr + 1.0;
		
		float zr = pow( r,Power);
		theta = theta*Power;
		phi = phi*Power;
		
		z = zr*vec3(sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta));
		z+=p;
	}
	return vec2(0.5*log(r)*r/dr, length(p) * i);
    /*
    int n,iters=int(Power);float t;
	for(n=0;n<iters;n++){
        p=abs(p);
		if(p.x<p.y){t=p.x;p.x=p.y;p.y=t;}
		if(p.y<p.z){t=p.y;p.y=p.z;p.z=t;}
		if(p.x<p.y){t=p.x;p.x=p.y;p.y=t;}
		p.x=p.x*3.0-2.0;p.y=p.y*3.0-2.0;p.z=p.z*3.0-2.0;
		if(p.z<-1.0)p.z+=2.0;
	}
	return vec2((sqrt(p.x*p.x+p.y*p.y+p.z*p.z)-1.5)*pow(3.0,-iters), 0);
    */

    //return vec2(length(p)-1, 0);
}

vec3 estimateNormal(vec3 p)
{
    float x = distToScene(vec3(p.x+epsilon,p.y,p.z)).x - distToScene(vec3(p.x-epsilon,p.y,p.z)).x;
    float y = distToScene(vec3(p.x,p.y+epsilon,p.z)).x - distToScene(vec3(p.x,p.y-epsilon,p.z)).x;
    float z = distToScene(vec3(p.x,p.y,p.z+epsilon)).x - distToScene(vec3(p.x,p.y,p.z-epsilon)).x;
    return normalize(vec3(x,y,z));
}

void main()
{
    vec2 screenPos;
    
    screenPos.x = gl_FragCoord.x/screensize.x - 0.5;
    screenPos.y = gl_FragCoord.y/screensize.y - 0.5;
    
    screenPos.x*=screensize.x/screensize.y;
    
    float zoom=1.0; 
    
    Ray camray = RayFromCamera(screenPos, Position, LookAt, zoom);
    
    float ray_distance = 0;
    float max_distance = 250;
    int steps = 0;
    int max_steps = 100;
    
    vec3 color_out = vec3(0,0,0);
    
    vec3 light_dir = vec3(0,0,1);
    
    vec2 dist;
    
    while (ray_distance < max_distance && steps < max_steps)
    {
        steps++;
        
        dist = distToScene(camray.pos);
        
        if (dist.x <= epsilon)
        {
            vec3 point = camray.pos + camray.dir * dist.x;
            vec3 normal = estimateNormal(point - camray.dir * epsilon);
            
            float colord = dot(normal * 0.5 + 0.5, -light_dir);
            
            color_out = vec3(-colord, -colord, -colord);
            break;
        }
        camray.pos += camray.dir * dist.x;
        ray_distance += dist.x;
    }
    

    
    color = vec4(color_out, 1.0);
}