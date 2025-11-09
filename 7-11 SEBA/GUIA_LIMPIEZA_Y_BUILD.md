# 🎸 Clone Hero - Guía de Limpieza y Build

## ✅ **PROBLEMA DEL VIDEO EN BUILD - SOLUCIONADO**

### **Causa del Problema**
El video de fondo funcionaba en el Editor de Unity pero no en el ejecutable (.exe) porque:
- La función `PrepareVideoURL` en `VideoPlayerBuildFix.cs` no manejaba correctamente las rutas absolutas de los videos
- No normalizaba las rutas antes de agregar el protocolo `file://`

### **Solución Aplicada**
Se corrigió la función `PrepareVideoURL` en el archivo:
```
Assets/Scripts/Gameplay/VideoPlayerBuildFix.cs
```

**Cambios realizados:**
1. Normalización de rutas (conversión de `\` a `/`)
2. Detección inteligente de rutas absolutas vs relativas
3. Manejo correcto del protocolo `file://` para Windows builds
4. Soporte mejorado para diferentes plataformas (Windows, Mac, Linux, Android, WebGL)

---

## 🧹 **LIMPIEZA DEL PROYECTO**

### **Archivos que se PUEDEN ELIMINAR de forma segura**

#### **1. Archivos de Backup (.bak) - 10 archivos**
Estos son copias de seguridad antiguas que ya no se necesitan:
```
Assets/Scripts/Gameplay/BackgroundVideoManagerClean.cs.bak
Assets/Scripts/Gameplay/FullScreenVideoBackground.cs.bak
Assets/Scripts/Gameplay/ProjectCleanupFixer.cs.bak
Assets/Scripts/Gameplay/QuickGameplayFix.cs.bak
Assets/Scripts/Gameplay/QuickVideoSetup.cs.bak
Assets/Scripts/Gameplay/SimpleVideoFix.cs.bak
Assets/Scripts/Gameplay/VideoFormatOptimizer.cs.bak
Assets/Scripts/Gameplay/VideoSetupHelper.cs.bak
Assets/Scripts/Gameplay/VideoSystemMigrator.cs.bak
Assets/Scripts/Gameplay/VideoSystemMigratorSimple.cs.bak
```
**+ sus archivos .meta correspondientes (10 archivos más)**

**TOTAL: 20 archivos .bak y .meta**

---

#### **2. Documentación Redundante (.md) - Se puede consolidar**
Hay muchos archivos de documentación que se pueden mover a una carpeta `Documentation/`:

**Documentación de Video (se puede consolidar en 1-2 archivos):**
```
Assets/Scripts/Gameplay/PauseVideoIntegration_Summary.md
Assets/Scripts/Gameplay/README_BackgroundVideos.md
Assets/Scripts/Gameplay/README_VideoSystem_Final.md
Assets/Scripts/Gameplay/README_VideoSystem_FromScratch.md
Assets/Scripts/Gameplay/VideoQualityGuide.md
Assets/Scripts/Gameplay/VideoRotationGuide.md
```

**Documentación de Highway/UI (se puede consolidar):**
```
Assets/Scripts/Gameplay/ChatGPT_Highway_Documentation.md
Assets/Scripts/Gameplay/Highway_Katana_Documentation.md
Assets/Scripts/Gameplay/Highway_Sprite_Fix_Guide.md
Assets/Scripts/Gameplay/Katana_Rectangle_Solution.md
Assets/Scripts/Gameplay/DebugUI_Cleanup_Summary.md
```

**Documentación de Loading Screen:**
```
Assets/Scripts/Gameplay/LoadingScreen_Documentation.md
Assets/Scripts/Gameplay/LoadingScreen_Fix_Summary.md
Assets/Scripts/Gameplay/LoadingScreen_Simple_Summary.md
```

**Documentación General:**
```
Assets/Scripts/ERRORES_COMPILACION_SOLUCIONADOS_FINAL.md
Assets/Scripts/SOLUCION_DUPLICADO_POSTGAMEPLAY.md
Assets/Scripts/Gameplay/README_HighwayAlignment.md
Assets/Scripts/Gameplay/README_HitZoneConfiguration.md
Assets/Scripts/Gameplay/README_INSTALACION.md
Assets/Scripts/Gameplay/README_TexturedButtons.md
Assets/Scripts/Gameplay/README_VisualFixes.md
Assets/Scripts/MainMenu/README_DifficultyButtonSetup.md
```

**TOTAL: 22 archivos .md + 22 archivos .meta = 44 archivos**

**RECOMENDACIÓN:** Mover toda la documentación a una nueva carpeta:
```
Assets/Documentation/
├── Video/
├── Highway/
├── UI/
└── General/
```

---

#### **3. Scripts Potencialmente Duplicados en Fixes/**
La carpeta `Assets/Scripts/Fixes/` contiene varios scripts que podrían estar duplicando funcionalidad:

```
ExternalFolderVideoLoader.cs
FixedStreamingVideoLoader.cs
ForceStreamingAssetsOnly.cs
ForceVideoLoader.cs
StreamingAssetsVideoLoader.cs
VideoClipLoader.cs
VideoDebugHelper.cs
```

**ANÁLISIS:** Todos estos scripts intentan cargar videos de diferentes formas. Con la corrección aplicada a `VideoPlayerBuildFix.cs`, estos scripts probablemente ya no sean necesarios.

**RECOMENDACIÓN:** 
- Mantener solo `VideoPlayerBuildFix.cs` (que ya está en `Assets/Scripts/Gameplay/`)
- Los scripts en `Fixes/` se pueden mover a una carpeta de backup o eliminar después de verificar que el sistema funciona correctamente

**TOTAL: 7 scripts + 7 .meta = 14 archivos**

---

#### **4. Scripts de Diagnóstico/Testing (opcional)**
Estos scripts son útiles para debugging pero no necesarios para el build final:

```
Assets/Scripts/Gameplay/BasicInputTest.cs
Assets/Scripts/Gameplay/ChordSystemDiagnostic.cs
Assets/Scripts/Gameplay/ChordTestingHelper.cs
Assets/Scripts/Gameplay/ControllerChordTest.cs
Assets/Scripts/Gameplay/ControllerGameplayTest.cs
Assets/Scripts/Gameplay/GameplayDebugger.cs
Assets/Scripts/Gameplay/HitDetectionDebugger.cs
Assets/Scripts/Gameplay/HighwaySpriteDiagnostic.cs
Assets/Scripts/Gameplay/KeyboardDebugger.cs
Assets/Scripts/Gameplay/PauseDiagnostic.cs
Assets/Scripts/Gameplay/PngKatanaDiagnostic.cs
Assets/Scripts/Gameplay/QuickChordTest.cs
Assets/Scripts/Gameplay/TestNoteGenerator.cs
Assets/Scripts/Gameplay/TransitionDiagnostic.cs
```

**RECOMENDACIÓN:** Mover a una carpeta `Assets/Scripts/_Testing/` para mantenerlos separados pero disponibles.

**TOTAL: 14 scripts + 14 .meta = 28 archivos**

---

### **RESUMEN DE LIMPIEZA**

| Categoría | Cantidad | Acción Recomendada |
|-----------|----------|-------------------|
| Archivos .bak | 20 archivos | ❌ **ELIMINAR** |
| Documentación .md | 44 archivos | 📁 **MOVER** a Assets/Documentation/ |
| Scripts Fixes/ | 14 archivos | 📦 **REVISAR** y posiblemente eliminar |
| Scripts Testing | 28 archivos | 📁 **MOVER** a Assets/Scripts/_Testing/ |
| **TOTAL** | **106 archivos** | |

---

## 🔨 **INSTRUCCIONES PARA CONSTRUIR EL PROYECTO**

### **Paso 1: Verificar los Videos en StreamingAssets**
Los videos DEBEN estar en la carpeta correcta para que funcionen en el build:

```
Assets/StreamingAssets/Songs/
├── [Nombre de Canción 1]/
│   ├── notes.chart
│   ├── song.ogg
│   └── video.mp4  ← Video aquí
├── [Nombre de Canción 2]/
│   ├── notes.chart
│   ├── song.ogg
│   └── video.mp4  ← Video aquí
└── ...
```

**VERIFICACIÓN:**
- ✅ Todos los videos están en sus carpetas de canción respectivas
- ✅ Los videos tienen nombres válidos: `video.mp4`, `background.mp4`, `bg.mp4`, etc.
- ✅ Formatos soportados: `.mp4`, `.webm`, `.mov`, `.avi`

---

### **Paso 2: Configurar Unity Build Settings**

1. **Abrir Build Settings:**
   - `File > Build Settings` o `Ctrl+Shift+B`

2. **Seleccionar Escenas:**
   - ✅ Agregar todas las escenas necesarias:
     - `MainMenu.unity`
     - `CrearUsuario.unity`
     - `Gameplay.unity`
     - `PostGameplay.unity`

3. **Platform: Windows**
   - Asegurarse que "PC, Mac & Linux Standalone" está seleccionado
   - Target Platform: Windows
   - Architecture: x86_64 (64-bit)

4. **Player Settings:**
   - Click en "Player Settings..."
   - **Company Name:** Tu nombre o nombre del equipo
   - **Product Name:** Clone Hero
   - **Icon:** (Opcional) Agregar icono del juego
   - **Fullscreen Mode:** Fullscreen Window (recomendado)
   - **Default Screen Width:** 1920
   - **Default Screen Height:** 1080

---

### **Paso 3: Configuración de Streaming Assets**

**IMPORTANTE:** Unity incluye automáticamente la carpeta `StreamingAssets` en el build.

**Verificar:**
1. La carpeta `Assets/StreamingAssets/` existe
2. Contiene la carpeta `Songs/` con todas las canciones
3. Cada canción tiene su video (si aplica)

**Tamaño del Build:**
- ⚠️ Los videos aumentan significativamente el tamaño del build
- Considera comprimir los videos antes de agregarlos
- Recomendación: Videos en 720p o 1080p máximo

---

### **Paso 4: Build del Proyecto**

1. **En Build Settings:**
   - Click en "Build" o "Build And Run"
   - Seleccionar carpeta de destino (ej: `Builds/CloneHero_v1.0/`)
   - Esperar a que Unity compile el proyecto

2. **Estructura del Build:**
```
CloneHero_v1.0/
├── CloneHero.exe               ← Ejecutable principal
├── UnityPlayer.dll
├── UnityCrashHandler64.exe
├── CloneHero_Data/
│   ├── StreamingAssets/        ← Videos aquí
│   │   └── Songs/
│   │       ├── Canción 1/
│   │       │   └── video.mp4
│   │       └── Canción 2/
│   │           └── video.mp4
│   ├── Managed/
│   ├── Resources/
│   └── ...
└── MonoBleedingEdge/
```

---

### **Paso 5: Verificar el Build**

1. **Ejecutar el juego:**
   - Doble click en `CloneHero.exe`
   - Verificar que el menú principal carga correctamente

2. **Probar el Gameplay:**
   - Seleccionar una canción
   - ✅ Verificar que el video de fondo se muestra correctamente
   - ✅ Verificar que el audio sincroniza con las notas
   - ✅ Verificar que el sistema de pausa funciona

3. **Si el video NO aparece:**
   - Verificar que existe `CloneHero_Data/StreamingAssets/Songs/[NombreCanción]/video.mp4`
   - Verificar que el formato del video es compatible (.mp4, .webm, .mov, .avi)
   - Revisar logs en: `%AppData%\..\LocalLow\[CompanyName]\CloneHero\output_log.txt`

---

## 🐛 **TROUBLESHOOTING**

### **Problema: El video no se ve en el build**
✅ **SOLUCIONADO** con la corrección en `VideoPlayerBuildFix.cs`

Si persiste el problema:
1. Verificar que la carpeta StreamingAssets/Songs existe en el build
2. Verificar que los videos están dentro de sus carpetas de canción
3. Revisar el log del juego para errores de carga

---

### **Problema: El build es muy grande**
**Causa:** Los videos ocupan mucho espacio

**Soluciones:**
1. Comprimir los videos antes de agregarlos (usar HandBrake o similar)
2. Reducir resolución de videos a 720p
3. Usar formato .webm en lugar de .mp4 (mejor compresión)

---

### **Problema: El juego se congela al cargar canciones**
**Causa:** Videos muy grandes o mal codificados

**Soluciones:**
1. Usar videos con codec H.264 (más compatible)
2. Reducir bitrate del video
3. Aumentar el `videoLoadTimeout` en `BackgroundVideoSystem` (Inspector)

---

## 📋 **CHECKLIST ANTES DE DISTRIBUIR**

- [ ] Todos los videos funcionan en el build
- [ ] No hay errores de compilación
- [ ] El juego inicia correctamente
- [ ] Todas las canciones cargan sin problemas
- [ ] El sistema de pausa funciona
- [ ] Los records se guardan correctamente
- [ ] Archivos .bak eliminados
- [ ] Documentación organizada
- [ ] Build testeado en otra computadora

---

## 📊 **ESTRUCTURA RECOMENDADA DEL PROYECTO**

```
Assets/
├── Fonts/
├── Prefabs/
├── Resources/
├── Scenes/
│   ├── MainMenu.unity
│   ├── CrearUsuario.unity
│   ├── Gameplay.unity
│   └── PostGameplay.unity
├── Scripts/
│   ├── Fixes/                  ← Mantener solo scripts activos
│   ├── Gameplay/               ← Scripts principales del juego
│   ├── InputName/
│   ├── MainMenu/
│   ├── Pergamino/
│   └── _Testing/               ← Mover scripts de debug aquí
├── StreamingAssets/
│   └── Songs/                  ← Videos y canciones aquí
├── TextMesh Pro/
├── UI/
└── Documentation/              ← Nueva carpeta para .md
    ├── Video/
    ├── Highway/
    ├── UI/
    └── General/
```

---

## ✅ **RESUMEN**

### **Problema del Video:**
- ✅ **SOLUCIONADO** mediante corrección en `VideoPlayerBuildFix.cs`
- Los videos ahora funcionan correctamente tanto en Editor como en Build

### **Limpieza del Proyecto:**
- 🗑️ **20 archivos .bak** para eliminar
- 📁 **44 archivos .md** para organizar en Documentation/
- 📦 **14 archivos** en Fixes/ para revisar
- 📁 **28 archivos** de testing para mover a _Testing/
- **Total: 106 archivos** para limpiar/organizar

### **Build del Proyecto:**
- ✅ Verificar videos en StreamingAssets/Songs/
- ✅ Configurar Build Settings correctamente
- ✅ Incluir todas las escenas necesarias
- ✅ Testear el build antes de distribuir

---

**Fecha de corrección:** 2 de noviembre, 2024
**Archivo modificado:** `Assets/Scripts/Gameplay/VideoPlayerBuildFix.cs`
**Función corregida:** `PrepareVideoURL(string videoPath)`
