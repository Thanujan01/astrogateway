import { Canvas, useFrame, useThree } from '@react-three/fiber'
import { Sky, Stars, useAnimations, useGLTF, useProgress, useTexture } from '@react-three/drei'
import { Suspense, useEffect, useLayoutEffect, useMemo, useRef, useState } from 'react'
import * as THREE from 'three'
import './App.css'

/* ---------------------------------------------------------------------------
 * Assets are the project's own art, staged into public/models by
 *   python tools/web-assets/prepare_web_assets.py
 * The suit is ARES_EVA_MK1_v3_Rigged: 1.86 m tall, Y-up, feet at origin, with
 * 16 in-place animation clips. It faces -Z, determined from the geometry --
 * the gold visor and safety-orange chest markings sit at negative Z while the
 * PLSS backpack extends to +Z.
 * ------------------------------------------------------------------------- */

const ASTRONAUT = '/models/astronaut.glb'
const ROVER = '/models/rover.glb'
const ROCK_A = '/models/rock_a.glb'
const ROCK_B = '/models/rock_b.glb'

useGLTF.preload(ASTRONAUT)
useGLTF.preload(ROVER)

const CLIPS = {
  idle: 'ARES_EVA_Idle_Breathing',
  walk: 'ARES_EVA_Walk_Forward',
  run: 'ARES_EVA_Run_Forward',
}

const keys: Record<string, boolean> = {}

// Orbit state for the follow camera, in radians / world units. The defaults reproduce the
// previous fixed (0, 5, 9) offset: distance 9.657 at pitch 0.371 puts the camera 3.5 above
// and 9 behind the look point, which sits 1.5 up from the astronaut's feet.
const view = { yaw: 0, pitch: 0.371, distance: 9.657, dragging: false }
const UP = new THREE.Vector3(0, 1, 0)
const LOOK_HEIGHT = 1.5
const PITCH_MIN = -0.35
const PITCH_MAX = 1.35
const DISTANCE_MIN = 4
const DISTANCE_MAX = 18
const SENSITIVITY = 0.0032

const WALK_SPEED = 2.6
const RUN_SPEED = 6
const PLAY_LIMIT = 45
// Movement is clamped per-axis, so the far corner sits 45*sqrt(2) = 63.6 out. Terrain stays
// flat to here and only undulates beyond it, which keeps the astronaut's feet planted
// everywhere it can actually walk.
const FLAT_RADIUS = 66
const GROUND_SIZE = 420

// Deterministic scatter: the same layout every load, with no dependency on Math.random.
function noise(i: number, seed: number) {
  const v = Math.sin(i * 127.1 + seed * 311.7) * 43758.5453
  return v - Math.floor(v)
}

/* --- Offline environment map ------------------------------------------------
 * Metals (the gold visor at metalness 0.98, titanium at 0.88) render black
 * without something to reflect. Rather than fetch an HDR from a CDN, bake a
 * Mars sky gradient into a PMREM cubemap locally so this works with no network.
 * ------------------------------------------------------------------------- */
function MarsEnvironment() {
  const gl = useThree(state => state.gl)

  const environment = useMemo(() => {
    const canvas = document.createElement('canvas')
    canvas.width = 32
    canvas.height = 256
    const ctx = canvas.getContext('2d')
    if (!ctx) return null

    // Stops are pre-dimmed to roughly 55% so the reflections read as a subtle sheen
    // rather than a second light source. Horizon haze sits at the 0.49/0.51 break.
    const gradient = ctx.createLinearGradient(0, 0, 0, canvas.height)
    gradient.addColorStop(0.0, '#3b3840')
    gradient.addColorStop(0.36, '#61432f')
    gradient.addColorStop(0.49, '#7d5d42')
    gradient.addColorStop(0.51, '#4e2919')
    gradient.addColorStop(0.72, '#3b1c12')
    gradient.addColorStop(1.0, '#22110a')
    ctx.fillStyle = gradient
    ctx.fillRect(0, 0, canvas.width, canvas.height)

    const source = new THREE.CanvasTexture(canvas)
    source.mapping = THREE.EquirectangularReflectionMapping
    source.colorSpace = THREE.SRGBColorSpace

    const pmrem = new THREE.PMREMGenerator(gl)
    const target = pmrem.fromEquirectangular(source)
    source.dispose()
    pmrem.dispose()
    return target.texture
  }, [gl])

  useEffect(() => () => environment?.dispose(), [environment])

  if (!environment) return null
  // Attaching declaratively sets scene.environment without mutating the hook's scene,
  // and R3F restores the previous value on unmount.
  return <primitive object={environment} attach="environment" />
}

/* --- Terrain --------------------------------------------------------------- */
function Terrain() {
  const maps = useTexture({
    map: '/models/ground_diffuse.jpg',
    normalMap: '/models/ground_normal.jpg',
    roughnessMap: '/models/ground_rough.jpg',
  })

  useLayoutEffect(() => {
    for (const [name, texture] of Object.entries(maps) as [string, THREE.Texture][]) {
      texture.wrapS = THREE.RepeatWrapping
      texture.wrapT = THREE.RepeatWrapping
      texture.repeat.set(110, 110)
      texture.anisotropy = 8
      texture.colorSpace = name === 'map' ? THREE.SRGBColorSpace : THREE.NoColorSpace
      texture.needsUpdate = true
    }
  }, [maps])

  const geometry = useMemo(() => {
    const geo = new THREE.PlaneGeometry(GROUND_SIZE, GROUND_SIZE, 200, 200)
    geo.rotateX(-Math.PI / 2)
    const position = geo.attributes.position as THREE.BufferAttribute
    for (let i = 0; i < position.count; i++) {
      const x = position.getX(i)
      const z = position.getZ(i)
      const radius = Math.hypot(x, z)
      // Zero across the whole walkable area, ramping in only past the play box.
      const falloff = THREE.MathUtils.smoothstep(radius, FLAT_RADIUS, GROUND_SIZE * 0.45)
      const height =
        Math.sin(x * 0.021) * Math.cos(z * 0.018) * 7.5 +
        Math.sin(x * 0.058 + 1.7) * Math.cos(z * 0.049) * 2.4 +
        Math.sin(x * 0.11 + 0.4) * Math.cos(z * 0.13) * 0.8
      position.setY(i, height * falloff)
    }
    geo.computeVertexNormals()
    return geo
  }, [])

  return (
    <mesh geometry={geometry} receiveShadow>
      <meshStandardMaterial
        {...maps}
        color="#b4714a"
        roughness={1}
        metalness={0}
        normalScale={new THREE.Vector2(1.1, 1.1)}
        dithering
      />
    </mesh>
  )
}

/* --- Distant ridges -------------------------------------------------------- */
function Ridges() {
  const mesas = useMemo(() => Array.from({ length: 22 }, (_, i) => {
    const angle = (i / 22) * Math.PI * 2 + noise(i, 3) * 0.2
    const radius = 150 + noise(i, 7) * 55
    const height = 14 + noise(i, 11) * 30
    const width = 34 + noise(i, 13) * 46
    return {
      position: [
        Math.sin(angle) * radius, height * 0.5 - 4, Math.cos(angle) * radius,
      ] as [number, number, number],
      scale: [
        width, height, width * (0.6 + noise(i, 17) * 0.5),
      ] as [number, number, number],
      rotation: noise(i, 19) * Math.PI,
    }
  }), [])

  return (
    <group>
      {mesas.map((mesa, i) => (
        <mesh key={i} position={mesa.position} scale={mesa.scale} rotation-y={mesa.rotation}>
          <dodecahedronGeometry args={[0.5, 0]} />
          <meshStandardMaterial color="#8a4c32" roughness={0.95} metalness={0} flatShading />
        </mesh>
      ))}
    </group>
  )
}

/* --- Rock field -----------------------------------------------------------
 * The rock GLBs are photogrammetry scans carrying their own PBR materials.
 * One InstancedMesh per scan keeps 94 boulders down to two draw calls.
 * ------------------------------------------------------------------------- */
function useScanGeometry(url: string) {
  const { scene } = useGLTF(url)
  return useMemo(() => {
    scene.updateMatrixWorld(true)
    let found: THREE.Mesh | undefined
    scene.traverse(object => {
      if (!found && (object as THREE.Mesh).isMesh) found = object as THREE.Mesh
    })
    if (!found) return null
    const geometry = found.geometry.clone().applyMatrix4(found.matrixWorld)
    geometry.computeBoundingBox()
    const box = geometry.boundingBox
    if (!box) return null
    const size = new THREE.Vector3()
    const centre = new THREE.Vector3()
    box.getSize(size)
    box.getCenter(centre)
    // Centre in XZ, sit it on y=0, then normalise to exactly 1 unit tall so that an
    // instance scale reads directly as metres.
    geometry.translate(-centre.x, -box.min.y, -centre.z)
    geometry.scale(1 / size.y, 1 / size.y, 1 / size.y)
    geometry.computeVertexNormals()
    return { geometry, material: found.material as THREE.Material }
  }, [scene])
}

function RockCluster({ url, count, seed, min, max }: {
  url: string
  count: number
  seed: number
  min: number
  max: number
}) {
  const scan = useScanGeometry(url)
  const ref = useRef<THREE.InstancedMesh>(null)

  useLayoutEffect(() => {
    const mesh = ref.current
    if (!mesh || !scan) return
    const matrix = new THREE.Matrix4()
    const quaternion = new THREE.Quaternion()
    const scale = new THREE.Vector3()
    const position = new THREE.Vector3()
    const euler = new THREE.Euler()
    let placed = 0

    for (let i = 0; placed < count && i < count * 6; i++) {
      const angle = noise(i, seed) * Math.PI * 2
      const radius = 5 + noise(i, seed + 1) * 95
      const x = Math.sin(angle) * radius
      const z = Math.cos(angle) * radius
      // Keep the spawn point and the habitat footprint clear.
      if (Math.hypot(x, z - 4) < 3.5) continue
      if (Math.hypot(x, z + 12) < 7.5) continue

      const height = min + noise(i, seed + 2) * (max - min)
      position.set(x, -height * 0.08, z)   // bed them slightly into the regolith
      euler.set(
        (noise(i, seed + 3) - 0.5) * 0.3,
        noise(i, seed + 4) * Math.PI * 2,
        (noise(i, seed + 5) - 0.5) * 0.3,
      )
      quaternion.setFromEuler(euler)
      scale.setScalar(height)
      mesh.setMatrixAt(placed, matrix.compose(position, quaternion, scale))
      placed++
    }

    mesh.count = placed
    mesh.instanceMatrix.needsUpdate = true
    mesh.computeBoundingSphere()
  }, [scan, count, seed, min, max])

  if (!scan) return null

  return (
    <instancedMesh
      ref={ref}
      args={[scan.geometry, scan.material, count]}
      castShadow
      receiveShadow
    />
  )
}

/* --- Rover ----------------------------------------------------------------
 * Perseverance_LOD2 carries deeply nested node scales (0.0009 up to 0.13), so
 * its real size is measured at runtime rather than assumed.
 * ------------------------------------------------------------------------- */
function Rover({ position, rotation }: {
  position: [number, number, number]
  rotation: number
}) {
  const { scene } = useGLTF(ROVER)

  const fit = useMemo(() => {
    scene.updateMatrixWorld(true)
    const box = new THREE.Box3().setFromObject(scene)
    const size = new THREE.Vector3()
    box.getSize(size)
    const scale = size.y > 0 ? 2.4 / size.y : 1
    return { scale, lift: -box.min.y * scale }
  }, [scene])

  useEffect(() => {
    scene.traverse(object => {
      const mesh = object as THREE.Mesh
      if (mesh.isMesh) {
        mesh.castShadow = true
        mesh.receiveShadow = true
        mesh.frustumCulled = false
      }
    })
  }, [scene])

  return (
    <group
      position={[position[0], position[1] + fit.lift, position[2]]}
      rotation-y={rotation}
      scale={fit.scale}
    >
      <primitive object={scene} />
    </group>
  )
}

/* --- Habitat --------------------------------------------------------------- */
function Habitat() {
  return (
    <group position={[0, 0, -12]}>
      {/* Pressurised module */}
      <mesh position={[0, 2, 0]} castShadow receiveShadow>
        <cylinderGeometry args={[4, 4, 4, 48]} />
        <meshStandardMaterial color="#cfd2c8" metalness={0.35} roughness={0.42} />
      </mesh>
      {/* Dome */}
      <mesh position={[0, 4, 0]} castShadow receiveShadow>
        <sphereGeometry args={[4, 48, 24, 0, Math.PI * 2, 0, Math.PI / 2]} />
        <meshStandardMaterial color="#dfe2d6" metalness={0.3} roughness={0.38} />
      </mesh>
      {/* Dust skirt where the hull meets regolith */}
      <mesh position={[0, 0.12, 0]} receiveShadow>
        <cylinderGeometry args={[4.5, 4.9, 0.24, 48]} />
        <meshStandardMaterial color="#8d5739" roughness={1} />
      </mesh>
      {/* Airlock */}
      <mesh position={[0, 1.3, 4.02]} castShadow>
        <boxGeometry args={[1.8, 2.5, 0.2]} />
        <meshStandardMaterial color="#2b3a46" metalness={0.65} roughness={0.35} />
      </mesh>
      <mesh position={[0, 2.32, 4.14]}>
        <boxGeometry args={[1, 0.26, 0.05]} />
        <meshStandardMaterial
          color="#8fe8ff"
          emissive="#38c6f4"
          emissiveIntensity={2.6}
          toneMapped={false}
        />
      </mesh>
      {/* Window band */}
      <mesh position={[0, 2.6, 0]}>
        <cylinderGeometry args={[4.02, 4.02, 0.7, 48, 1, true]} />
        <meshStandardMaterial
          color="#1a2b38"
          metalness={0.9}
          roughness={0.08}
          side={THREE.DoubleSide}
        />
      </mesh>
      <pointLight position={[0, 2.4, 4.6]} intensity={6} distance={9} color="#7fd8ff" />
    </group>
  )
}

function SolarPanel({ position, tilt }: {
  position: [number, number, number]
  tilt: number
}) {
  return (
    <group position={position} rotation-y={tilt}>
      <mesh position={[0, 0.8, 0]} castShadow>
        <cylinderGeometry args={[0.07, 0.11, 1.6, 16]} />
        <meshStandardMaterial color="#9ea19c" metalness={0.75} roughness={0.35} />
      </mesh>
      <mesh position={[0, 1.65, 0]} rotation={[-0.42, 0, 0]} castShadow receiveShadow>
        <boxGeometry args={[4, 0.07, 2]} />
        <meshStandardMaterial color="#1b3f6b" metalness={0.85} roughness={0.16} />
      </mesh>
      {/* Cell-grid highlight */}
      <mesh position={[0, 1.69, 0]} rotation={[-0.42, 0, 0]}>
        <boxGeometry args={[3.86, 0.01, 1.86]} />
        <meshStandardMaterial color="#2d6ab5" metalness={0.95} roughness={0.08} />
      </mesh>
    </group>
  )
}

/* --- Astronaut ------------------------------------------------------------- */
function Astronaut() {
  const root = useRef<THREE.Group>(null)
  const camera = useThree(state => state.camera)
  const canvas = useThree(state => state.gl.domElement)
  const { scene, animations } = useGLTF(ASTRONAUT)
  const { actions } = useAnimations(animations, root)
  const active = useRef('')

  const fit = useMemo(() => {
    scene.updateMatrixWorld(true)
    const box = new THREE.Box3().setFromObject(scene)
    const size = new THREE.Vector3()
    box.getSize(size)
    // Authored at 1.86 m; normalise anyway so a re-export cannot silently change scale.
    return size.y > 0 ? 1.82 / size.y : 1
  }, [scene])

  useEffect(() => {
    scene.traverse(object => {
      const mesh = object as THREE.Mesh
      if (mesh.isMesh) {
        mesh.castShadow = true
        mesh.receiveShadow = true
        // Skinned bounds come from the bind pose and can cull incorrectly.
        mesh.frustumCulled = false
      }
    })
  }, [scene])

  useEffect(() => {
    const down = (e: KeyboardEvent) => { keys[e.code] = true }
    const up = (e: KeyboardEvent) => { keys[e.code] = false }
    // Without this, a key held while the tab loses focus never gets its keyup and the
    // astronaut keeps walking after the user comes back.
    const clear = () => {
      for (const code of Object.keys(keys)) keys[code] = false
      view.dragging = false
    }
    window.addEventListener('keydown', down)
    window.addEventListener('keyup', up)
    window.addEventListener('blur', clear)
    return () => {
      window.removeEventListener('keydown', down)
      window.removeEventListener('keyup', up)
      window.removeEventListener('blur', clear)
      clear()
    }
  }, [])

  // Mouse / touch look. Listeners live on the canvas so the HUD stays clickable, and the
  // drag is pointer-captured so it keeps tracking when the cursor leaves the canvas.
  useEffect(() => {
    let pointer: number | null = null
    let lastX = 0
    let lastY = 0

    const start = (e: PointerEvent) => {
      if (pointer !== null) return
      pointer = e.pointerId
      lastX = e.clientX
      lastY = e.clientY
      view.dragging = true
      canvas.setPointerCapture(e.pointerId)
      e.preventDefault()
    }
    const move = (e: PointerEvent) => {
      if (pointer !== e.pointerId) return
      // movementX/Y is unavailable on touch and on some browsers mid-capture, so fall
      // back to the delta between events.
      const dx = e.movementX ?? e.clientX - lastX
      const dy = e.movementY ?? e.clientY - lastY
      lastX = e.clientX
      lastY = e.clientY
      view.yaw -= dx * SENSITIVITY
      view.pitch = THREE.MathUtils.clamp(view.pitch + dy * SENSITIVITY, PITCH_MIN, PITCH_MAX)
    }
    const end = (e: PointerEvent) => {
      if (pointer !== e.pointerId) return
      pointer = null
      view.dragging = false
      if (canvas.hasPointerCapture(e.pointerId)) canvas.releasePointerCapture(e.pointerId)
    }
    const zoom = (e: WheelEvent) => {
      view.distance = THREE.MathUtils.clamp(
        view.distance + Math.sign(e.deltaY) * 0.9, DISTANCE_MIN, DISTANCE_MAX,
      )
      e.preventDefault()
    }
    // Suppress the context menu so right-drag can orbit, matching the Unity prototype.
    const noMenu = (e: MouseEvent) => e.preventDefault()

    canvas.addEventListener('pointerdown', start)
    canvas.addEventListener('pointermove', move)
    canvas.addEventListener('pointerup', end)
    canvas.addEventListener('pointercancel', end)
    canvas.addEventListener('wheel', zoom, { passive: false })
    canvas.addEventListener('contextmenu', noMenu)
    return () => {
      canvas.removeEventListener('pointerdown', start)
      canvas.removeEventListener('pointermove', move)
      canvas.removeEventListener('pointerup', end)
      canvas.removeEventListener('pointercancel', end)
      canvas.removeEventListener('wheel', zoom)
      canvas.removeEventListener('contextmenu', noMenu)
      view.dragging = false
    }
  }, [canvas])

  // Crossfade between clips. Called from useFrame, so this deliberately uses a ref rather
  // than React state: a setState per frame would re-render the whole scene.
  function play(name: keyof typeof CLIPS, timeScale: number) {
    const clip = CLIPS[name]
    const next = actions[clip]
    if (!next) return
    next.timeScale = timeScale
    if (active.current === clip) return
    const previous = actions[active.current]
    next.reset().setEffectiveWeight(1).fadeIn(0.22).play()
    if (previous) previous.fadeOut(0.22)
    active.current = clip
  }

  useEffect(() => {
    const idle = actions[CLIPS.idle]
    if (idle && !active.current) {
      idle.reset().play()
      active.current = CLIPS.idle
    }
  }, [actions])

  useFrame((_, delta) => {
    const group = root.current
    if (!group) return

    const sprinting = Boolean(keys.ShiftLeft || keys.ShiftRight)
    const speed = sprinting ? RUN_SPEED : WALK_SPEED
    const direction = new THREE.Vector3()

    if (keys.KeyW || keys.ArrowUp) direction.z -= 1
    if (keys.KeyS || keys.ArrowDown) direction.z += 1
    if (keys.KeyA || keys.ArrowLeft) direction.x -= 1
    if (keys.KeyD || keys.ArrowRight) direction.x += 1

    const step = Math.min(delta, 0.05)

    if (direction.lengthSq() > 0) {
      // Movement is relative to where the camera is looking, so W always walks away from
      // the camera. At yaw 0 this is identical to the previous world-axis behaviour.
      direction.normalize().applyAxisAngle(UP, view.yaw)
      group.position.addScaledVector(direction, speed * step)

      // Turn toward travel rather than snapping. The suit faces -Z, hence the negations.
      const target = Math.atan2(-direction.x, -direction.z)
      const shortest = Math.atan2(
        Math.sin(target - group.rotation.y),
        Math.cos(target - group.rotation.y),
      )
      group.rotation.y += shortest * (1 - Math.exp(-14 * delta))

      play(sprinting ? 'run' : 'walk', 1)
    } else {
      play('idle', 1)
    }

    group.position.x = THREE.MathUtils.clamp(group.position.x, -PLAY_LIMIT, PLAY_LIMIT)
    group.position.z = THREE.MathUtils.clamp(group.position.z, -PLAY_LIMIT, PLAY_LIMIT)

    const look = group.position.clone()
    look.y += LOOK_HEIGHT
    const cos = Math.cos(view.pitch)
    const wanted = look.clone().add(new THREE.Vector3(
      Math.sin(view.yaw) * cos,
      Math.sin(view.pitch),
      Math.cos(view.yaw) * cos,
    ).multiplyScalar(view.distance))
    // Keep the camera above the ground plane however far down the user orbits.
    wanted.y = Math.max(wanted.y, 0.6)

    if (view.dragging) {
      // Snap while the user is actively looking around; lag here reads as broken controls.
      camera.position.copy(wanted)
    } else {
      camera.position.lerp(wanted, 1 - Math.exp(-9 * delta))
    }
    camera.lookAt(look)
  })

  return (
    <group ref={root} position={[0, 0, 4]}>
      <primitive object={scene} scale={fit} />
    </group>
  )
}

/* --- Scene ----------------------------------------------------------------- */
function MarsWorld() {
  const sun = useMemo(() => new THREE.Vector3(58, 26, -34), [])

  return (
    <>
      <fogExp2 attach="fog" args={['#c07a52', 0.0062]} />
      <Sky
        distance={4000}
        sunPosition={sun}
        turbidity={13}
        rayleigh={0.55}
        mieCoefficient={0.022}
        mieDirectionalG={0.86}
      />
      <Stars radius={320} depth={60} count={1400} factor={3} fade saturation={0} />

      <MarsEnvironment />

      {/* Mars receives roughly 43% of Earth's irradiance, so the sun reads bright but small
          and most of the fill comes from red dust bounce. */}
      <hemisphereLight args={['#e9a978', '#5d2a18', 0.75]} />
      <ambientLight intensity={0.1} />
      <directionalLight
        position={sun}
        intensity={2.5}
        color="#fff0dc"
        castShadow
        shadow-mapSize={[2048, 2048]}
        shadow-bias={-0.0004}
        shadow-normalBias={0.025}
        shadow-camera-near={1}
        shadow-camera-far={220}
        shadow-camera-left={-70}
        shadow-camera-right={70}
        shadow-camera-top={70}
        shadow-camera-bottom={-70}
      />

      <Terrain />
      <Ridges />

      <Habitat />
      <SolarPanel position={[-9, 0, -8]} tilt={0.3} />
      <SolarPanel position={[9, 0, -8]} tilt={-0.3} />

      {/* The astronaut (the player's own avatar) stays in the outer Suspense so it appears
          as soon as its own asset resolves. The rover and rock field are background set
          dressing -- a nested boundary lets them stream in afterward instead of making the
          player wait on every asset before anything is interactive. */}
      <Astronaut />

      <Suspense fallback={null}>
        <RockCluster url={ROCK_A} count={60} seed={1} min={0.35} max={1.9} />
        <RockCluster url={ROCK_B} count={34} seed={2} min={0.18} max={0.7} />
        <Rover position={[6.5, 0, -2]} rotation={-0.7} />
      </Suspense>
    </>
  )
}

function LoadingVeil() {
  const { active, progress } = useProgress()
  if (!active) return null
  return (
    <div className="loading">
      <strong>PREPARING SURFACE OPERATIONS</strong>
      <div className="loading-bar">
        <span style={{ width: progress.toFixed(1) + '%' }} />
      </div>
      <small>{progress.toFixed(0)}% · loading suit, rover and terrain</small>
    </div>
  )
}

function App() {
  const [showHelp, setShowHelp] = useState(true)

  return (
    <main className="game">
      <Canvas
        shadows
        dpr={[1, 2]}
        camera={{ position: [0, 5, 13], fov: 60, near: 0.1, far: 2200 }}
        gl={{ antialias: true, powerPreference: 'high-performance' }}
        onCreated={({ gl }) => {
          gl.toneMapping = THREE.ACESFilmicToneMapping
          gl.toneMappingExposure = 1.08
        }}
      >
        <Suspense fallback={null}>
          <MarsWorld />
        </Suspense>
      </Canvas>

      <LoadingVeil />

      <div className="hud-top">
        <div className="brand">
          <strong>ASTROGATEWAY</strong>
          <span>MISSION RED HORIZON</span>
        </div>
        <div className="resources">
          <span>O₂ 72%</span>
          <span>⚡ 60%</span>
          <span>💧 61%</span>
          <span>☢ LOW</span>
        </div>
      </div>

      <div className="mission">
        <small>MISSION 01 · SOL 01</small>
        <h2>Explore the Mars Outpost</h2>
        <p>Approach the habitat and inspect the base.</p>
      </div>

      {showHelp && (
        <div className="help">
          <button onClick={() => setShowHelp(false)}>✕</button>
          <h3>ASTRONAUT CONTROLS</h3>
          <p>Drag Mouse — Look around</p>
          <p>W A S D — Move</p>
          <p>Shift — Sprint</p>
          <p>Mouse Wheel — Zoom</p>
          <small>
            Movement follows your view: W always walks the way the camera faces.
          </small>
        </div>
      )}

      <div className="bottom-label">
        SK2LB-AstroLab · Junior Astronaut Mission Trainer
      </div>
    </main>
  )
}

export default App
