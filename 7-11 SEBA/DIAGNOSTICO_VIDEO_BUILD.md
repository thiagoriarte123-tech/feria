# 🔍 Diagnóstico del Problema de Video en Build

## ✅ Cambios Aplicados (Segunda Iteración)

He realizado más correcciones al código para solucionar el problema del video y **agregar logs detallados** que nos ayudarán a diagnosticar exactamente qué está pasando.

### Archivos Modificados:

1. **`VideoPlayerBuildFix.cs`**
   - ✅ Corregida lógica de carga para manejar rutas absolutas correctamente
   - ✅ Agregados logs detallados en todas las funciones clave
   - ✅ Ahora busca el video directamente en la carpeta proporcionada (sin importar si es Editor o Build)

2. **`BackgroundVideoSystem.cs`**
   - ✅ Activados logs de debug por defecto
   - ✅ Agregados logs detallados en LoadVideoAsync

---

## 🎯 **PASOS PARA DIAGNOSTICAR EL PROBLEMA**

### **Paso 1: Hacer un Build de Prueba**

1. Abre Unity Editor
2. Ve a: `File > Build Settings`
3. Asegúrate que estas escenas estén incluidas:
   - MainMenu.unity
   - CrearUsuario.unity
   - Gameplay.unity
   - PostGameplay.unity
4. Click en **Build** (NO "Build and Run")
5. Guarda el build en una carpeta (ej: `Builds/Test_Video/`)

---

### **Paso 2: Verificar que los Videos Estén en el Build**

Después de hacer el build, verifica que los videos se copiaron:

```
Builds/Test_Video/
└── CloneHero_Data/
    └── StreamingAssets/
        └── Songs/
            ├── [Nombre Canción 1]/
            │   └── video.mp4          ← ¿Existe?
            ├── [Nombre Canción 2]/
            │   └── video.mp4          ← ¿Existe?
            └── ...
```

**SI LOS VIDEOS NO ESTÁN AHÍ:**
- ❌ El problema es que los videos no se están copiando al build
- Verifica que los videos estén en: `Assets/StreamingAssets/Songs/[NombreCanción]/video.mp4`

**SI LOS VIDEOS SÍ ESTÁN AHÍ:**
- ✅ Los videos se copiaron correctamente
- El problema es de carga, continúa al Paso 3

---

### **Paso 3: Ver los Logs del Juego**

Los logs del juego en Windows se guardan en:

```
%USERPROFILE%\AppData\LocalLow\[CompanyName]\CloneHero\Player.log
```

**Para encontrar el archivo fácilmente:**

1. Presiona `Win + R`
2. Escribe: `%USERPROFILE%\AppData\LocalLow`
3. Busca la carpeta de tu juego (según el Company Name que configuraste)
4. Abre el archivo `Player.log` con Notepad o tu editor de texto

**O usa este comando PowerShell:**
```powershell
cd "$env:USERPROFILE\AppData\LocalLow"
dir -Recurse Player.log | select FullName
```

---

### **Paso 4: Ejecutar el Juego y Leer los Logs**

1. **Ejecuta el juego** (CloneHero.exe)
2. **Juega una canción** que tenga video
3. **Cierra el juego**
4. **Abre el archivo Player.log**

---

### **Paso 5: Buscar los Logs del Video**

En el archivo `Player.log`, busca estas líneas (usa Ctrl+F):

**Busca:** `🎬`

Deberías ver logs como estos:

```
🎬 BackgroundVideoSystem: Iniciando carga de video para: [ruta]
🎬 Application.isEditor: False
🎬 Application.streamingAssetsPath: [ruta del build]
🎬 LoadVideoCoroutine iniciado para: [ruta]
🎬 FindVideoInFolder: Buscando en [ruta]
```

---

## 🔍 **INTERPRETACIÓN DE LOS LOGS**

### **Caso 1: "FindVideoInFolder: Carpeta NO existe"**

```
🎬 FindVideoInFolder: Carpeta NO existe: C:/...
```

**Problema:** La ruta que se está buscando no existe.

**Soluciones posibles:**
- Verifica que `Application.streamingAssetsPath` apunte a la ubicación correcta
- Verifica que la carpeta `StreamingAssets/Songs/[NombreCanción]` existe en el build
- Puede ser un problema con cómo se está construyendo la ruta

---

### **Caso 2: "No se encontró ningún video en la carpeta"**

```
🎬 FindVideoInFolder: ❌ No se encontró ningún video en la carpeta
```

**Problema:** La carpeta existe pero no tiene archivos .mp4, .webm, .mov o .avi

**Soluciones:**
- Verifica que el video tenga uno de los formatos soportados
- Verifica que el nombre del archivo sea correcto (video.mp4, background.mp4, etc.)
- Puede ser un problema de permisos de archivo

---

### **Caso 3: "Video encontrado" pero luego error**

```
🎬 FindVideoInFolder: ✅ Video encontrado: [ruta]
🎬 PrepareVideoURL - Input: [ruta]
🎬 PrepareVideoURL - Final URL: file://[ruta]
🎬 Cargando video: file://[ruta]
🎬 Error cargando video: [mensaje de error]
```

**Problema:** El video se encontró pero hay un error al cargarlo

**Posibles causas:**
- Formato de video no compatible con Unity VideoPlayer
- Codec del video no soportado
- Archivo de video corrupto
- URL mal formada

**Soluciones:**
- Reconvertir el video a MP4 con codec H.264
- Reducir la resolución del video
- Probar con otro archivo de video

---

### **Caso 4: "Error o timeout cargando video"**

```
❌ Error o timeout cargando video (timer: 8.0s, loadCompleted: false, loadSuccess: false)
```

**Problema:** El video tardó más de 8 segundos en cargar (timeout)

**Soluciones:**
1. Aumentar el timeout en Unity:
   - Abre la escena Gameplay
   - Selecciona el GameObject con BackgroundVideoSystem
   - En el Inspector, aumenta `Video Load Timeout` a 15 o 20 segundos

2. Comprimir el video:
   - Reducir la resolución a 720p
   - Reducir el bitrate
   - Usar formato .webm

---

## 🛠️ **SOLUCIONES RÁPIDAS**

### **Solución 1: Verificar Company Name**

El Company Name afecta dónde se guardan los logs.

1. En Unity: `Edit > Project Settings > Player`
2. Verifica el **Company Name**
3. Los logs estarán en: `%USERPROFILE%\AppData\LocalLow\[CompanyName]\CloneHero\Player.log`

---

### **Solución 2: Reconvertir Videos**

Si los videos no cargan, reconviértelos con estos settings:

**Usando HandBrake (gratuito):**
- Container: MP4
- Video Codec: H.264
- Framerate: Same as source
- Resolution: 1280x720 o 1920x1080
- Bitrate: 2000-3000 kbps
- Audio: Eliminar audio (o AAC 128kbps si quieres mantenerlo)

**Nombre del archivo:** `video.mp4`
**Ubicación:** `Assets/StreamingAssets/Songs/[NombreCanción]/video.mp4`

---

### **Solución 3: Probar con un Video Simple**

Descarga un video de prueba simple y pequeño:

1. Busca un video corto (10-30 segundos) en YouTube
2. Descárgalo en 720p
3. Conviértelo a MP4 con HandBrake
4. Renómbralo a `video.mp4`
5. Cópialo a la carpeta de una canción
6. Haz el build y prueba

---

## 📋 **CHECKLIST DE VERIFICACIÓN**

Antes de reportar el problema, verifica:

- [ ] Los videos están en `Assets/StreamingAssets/Songs/[Canción]/video.mp4`
- [ ] Los videos existen en el build (`CloneHero_Data/StreamingAssets/Songs/`)
- [ ] El formato es MP4, WEBM, MOV o AVI
- [ ] El video no está corrupto (se puede reproducir en VLC)
- [ ] Has leído el archivo Player.log
- [ ] Has buscado los logs con 🎬
- [ ] El Company Name está configurado correctamente

---

## 📊 **INFORMACIÓN NECESARIA PARA AYUDARTE**

Si el problema persiste, necesito esta información:

1. **Contenido del Player.log** (las líneas que tienen 🎬)
2. **Ruta del build** (ej: `C:/Builds/CloneHero/`)
3. **Nombre de la canción que probaste**
4. **¿El video existe en el build?** (Sí/No)
5. **Formato y tamaño del video** (ej: MP4, 50MB, 1920x1080)
6. **Company Name** configurado en Unity

---

## 🎯 **PRÓXIMOS PASOS**

1. ✅ Hacer un build de prueba
2. ✅ Verificar que los videos se copiaron al build
3. ✅ Ejecutar el juego y probar una canción
4. ✅ Abrir Player.log y buscar los logs 🎬
5. ✅ Enviarme los logs que encontraste

Con esta información podré identificar exactamente qué está fallando y darte una solución específica.

---

**Fecha:** 2 de noviembre, 2024  
**Versión:** Diagnóstico v2 con logs detallados  
**Archivos modificados:** VideoPlayerBuildFix.cs, BackgroundVideoSystem.cs
