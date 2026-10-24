
import { Canvas, useFrame, useThree } from '@react-three/fiber'
import { Sky, Stars } from '@react-three/drei'
import { useEffect, useRef, useState } from 'react'
import * as THREE from 'three'
import './App.css'

const keys: Record<string, boolean> = {}

function Astronaut() {
  const player = useRef<THREE.Group>(null)
  const { camera } = useThree()

  useEffect(() => {
    const down = (e: KeyboardEvent) => {
      keys[e.code] = true
    }
    const up = (e: KeyboardEvent) => {
      keys[e.code] = false
    }

    window.addEventListener('keydown', down)
    window.addEventListener('keyup', up)

    return () => {
      window.removeEventListener('keydown', down)
      window.removeEventListener('keyup', up)
    }
  }, [])

  useFrame((_, delta) => {
    if (!player.current) return

    const speed = keys.ShiftLeft ? 8 : 4
    const direction = new THREE.Vector3()

    if (keys.KeyW || keys.ArrowUp) direction.z -= 1
    if (keys.KeyS || keys.ArrowDown) direction.z += 1
    if (keys.KeyA || keys.ArrowLeft) direction.x -= 1
    if (keys.KeyD || keys.ArrowRight) direction.x += 1

    if (direction.lengthSq() > 0) {
      direction.normalize()
      player.current.position.addScaledVector(
        direction,
        speed * Math.min(delta, 0.05)
      )
      player.current.rotation.y = Math.atan2(
        -direction.x,
        -direction.z
      )
    }

    player.current.position.x = THREE.MathUtils.clamp(
      player.current.position.x, -45, 45
    )
    player.current.position.z = THREE.MathUtils.clamp(
      player.current.position.z, -45, 45
    )

    const target = player.current.position.clone()
    const cameraPosition = target.clone().add(
      new THREE.Vector3(0, 5, 9)
    )

    camera.position.lerp(cameraPosition, 0.08)
    camera.lookAt(target.clone().add(new THREE.Vector3(0, 1.5, 0)))
  })

  return (
    <group ref={player} position={[0, 0, 4]}>
      {/* Spacesuit */}
      <mesh position={[0, 1.15, 0]} castShadow>
        <capsuleGeometry args={[0.4, 0.85, 8, 16]} />
        <meshStandardMaterial color="#e6e9e8" />
      </mesh>

      {/* Helmet */}
      <mesh position={[0, 2.05, 0]} castShadow>
        <sphereGeometry args={[0.43, 24, 24]} />
        <meshStandardMaterial color="#d9e4e9" metalness={0.3} />
      </mesh>

      {/* Visor */}
      <mesh position={[0, 2.07, -0.32]}>
        <sphereGeometry args={[0.29, 24, 24]} />
        <meshStandardMaterial
          color="#183e56"
          metalness={0.7}
          roughness={0.2}
        />
      </mesh>

      {/* Backpack */}
      <mesh position={[0, 1.25, 0.48]} castShadow>
        <boxGeometry args={[0.7, 0.95, 0.35]} />
        <meshStandardMaterial color="#c2c9c9" />
      </mesh>

      {/* Legs */}
      {[-0.22, 0.22].map((x) => (
        <mesh key={x} position={[x, 0.42, 0]} castShadow>
          <capsuleGeometry args={[0.16, 0.45, 6, 12]} />
          <meshStandardMaterial color="#dce1df" />
        </mesh>
      ))}
    </group>
  )
}

function Habitat() {
  return (
    <group position={[0, 0, -12]}>
      <mesh position={[0, 2, 0]} castShadow receiveShadow>
        <cylinderGeometry args={[4, 4, 4, 32]} />
        <meshStandardMaterial color="#c4c7bd" metalness={0.3} />
      </mesh>

      <mesh position={[0, 4.2, 0]} castShadow>
        <sphereGeometry args={[4, 32, 16, 0, Math.PI * 2, 0, Math.PI / 2]} />
        <meshStandardMaterial color="#d5d8cc" />
      </mesh>

      <mesh position={[0, 1.3, 4.02]}>
        <boxGeometry args={[1.8, 2.5, 0.15]} />
        <meshStandardMaterial color="#263b4c" />
      </mesh>

      <mesh position={[0, 2.3, 4.12]}>
        <boxGeometry args={[1, 0.3, 0.05]} />
        <meshStandardMaterial
          color="#46d6ff"
          emissive="#087cbb"
          emissiveIntensity={1.5}
        />
      </mesh>
    </group>
  )
}

function SolarPanel({ position }: { position: [number, number, number] }) {
  return (
    <group position={position}>
      <mesh position={[0, 0.8, 0]}>
        <cylinderGeometry args={[0.07, 0.1, 1.6]} />
        <meshStandardMaterial color="#9b9b9b" />
      </mesh>
      <mesh position={[0, 1.65, 0]} rotation={[-0.35, 0, 0]}>
        <boxGeometry args={[4, 0.08, 2]} />
        <meshStandardMaterial
          color="#183d68"
          metalness={0.7}
          roughness={0.25}
        />
      </mesh>
    </group>
  )
}

function MarsWorld() {
  return (
    <>
      <color attach="background" args={['#9a5139']} />
      <fog attach="fog" args={['#b66d4b', 35, 115]} />

      <ambientLight intensity={1.4} />
      <directionalLight
        position={[20, 30, 10]}
        intensity={2.4}
        castShadow
        shadow-mapSize={[1024, 1024]}
      />

      <Sky sunPosition={[10, 3, -20]} turbidity={9} rayleigh={3} />
      <Stars radius={120} depth={40} count={700} factor={2} />

      <mesh rotation={[-Math.PI / 2, 0, 0]} receiveShadow>
        <planeGeometry args={[200, 200]} />
        <meshStandardMaterial color="#a64d30" roughness={1} />
      </mesh>

      <Habitat />
      <SolarPanel position={[-9, 0, -8]} />
      <SolarPanel position={[9, 0, -8]} />

      {Array.from({ length: 45 }, (_, i) => {
        const x = Math.sin(i * 13.7) * 38
        const z = Math.cos(i * 8.3) * 38
        const size = 0.2 + (i % 5) * 0.16

        return (
          <mesh
            key={i}
            position={[x, size / 2, z]}
            rotation={[i, i * 0.7, i * 0.2]}
            castShadow
          >
            <dodecahedronGeometry args={[size, 0]} />
            <meshStandardMaterial color="#77412f" />
          </mesh>
        )
      })}

      <Astronaut />
    </>
  )
}

function App() {
  const [showHelp, setShowHelp] = useState(true)

  return (
    <main className="game">
      <Canvas shadows camera={{ position: [0, 5, 13], fov: 65 }}>
        <MarsWorld />
      </Canvas>

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
          <p>W A S D — Move</p>
          <p>Shift — Sprint</p>
          <p>Arrow Keys — Move</p>
          <small>Camera follows your astronaut automatically.</small>
        </div>
      )}

      <div className="bottom-label">
        SK2LB-AstroLab · Junior Astronaut Mission Trainer
      </div>
    </main>
  )
}

export default App
