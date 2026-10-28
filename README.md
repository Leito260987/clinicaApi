# Clínica – API RESTful de Citas Médicas

API desarrollada en **C# con .NET 8** para registrar, consultar, reprogramar, cancelar y eliminar citas médicas de una clínica privada. Reemplaza el control manual en agendas físicas y llamadas telefónicas por un servicio único y ordenado.

Los datos se guardan **en memoria** (`List<T>`), sin base de datos: la API simula el comportamiento de una base de datos real (identificadores autoincrementales, unicidad, integridad referencial y acceso concurrente seguro), pero la información se pierde al detener la aplicación.

> Proyecto académico – Evaluación parcial del curso **Desarrollo de Servicios Web**, ZEGEL (Escuela de Tecnología).

---

## Contenido

1. [Tecnologías](#tecnologías)
2. [Requisitos](#requisitos)
3. [Instalación y ejecución](#instalación-y-ejecución)
4. [Estructura del proyecto](#estructura-del-proyecto)
5. [Modelo de dominio y reglas de negocio](#modelo-de-dominio-y-reglas-de-negocio)
6. [Endpoints](#endpoints)
7. [Formato de respuestas y códigos HTTP](#formato-de-respuestas-y-códigos-http)
8. [Ejemplos de uso](#ejemplos-de-uso)
9. [Pruebas con Postman](#pruebas-con-postman)
10. [Equipo y roles](#equipo-y-roles)

---

## Tecnologías

| Tecnología | Uso |
| --- | --- |
| C# 12 / .NET 8 (ASP.NET Core Web API) | Lenguaje y framework de la API |
| Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[RegularExpression]`, `[EmailAddress]`) | Validación de datos de entrada |
| Swagger / OpenAPI (Swashbuckle 6.6.2) | Documentación interactiva de los endpoints |
| Postman + Newman | Pruebas funcionales automatizadas |

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) o superior. Si solo tienes .NET 9 o .NET 10, también funciona: el proyecto usa `RollForward=Major`.
- [Postman](https://www.postman.com/downloads/) para ejecutar la colección de pruebas (opcional: Newman por consola).
- Git.

Comprueba la instalación con:

```bash
dotnet --version
```

## Instalación y ejecución

```bash
# 1. Clonar el repositorio
git clone https://github.com/Leito260987/clinicaApi.git
cd clinica-citas-api

# 2. Restaurar dependencias (descarga Swashbuckle desde NuGet)
dotnet restore

# 3. Ejecutar la API
dotnet run --project src/ClinicaCitas.Api
```

La API queda disponible en **http://localhost:5080**:

| Recurso | URL |
| --- | --- |
| Documentación Swagger | http://localhost:5080/swagger |
| Ejemplo de endpoint | http://localhost:5080/api/citas |

**Con Visual Studio 2022:** abre `ClinicaCitas.sln`, elige el perfil **http** y presiona **F5**; Swagger se abre automáticamente.

**Con perfil HTTPS:** `dotnet run --project src/ClinicaCitas.Api --launch-profile https` (https://localhost:7080). La primera vez puede pedir `dotnet dev-certs https --trust`.

Al iniciar se cargan **datos de ejemplo**: 3 pacientes, 3 médicos y 3 citas en los próximos días hábiles (una de ellas cancelada), para poder probar la API de inmediato.

## Estructura del proyecto

```
clinica-citas-api/
├── ClinicaCitas.sln
├── README.md
├── postman/                         Colección y environment de Postman
├── docs/
│   ├── capturas/                    Capturas de la ejecución de las pruebas
│   └── pruebas/reporte-newman.html  Reporte HTML de Newman
└── src/ClinicaCitas.Api/
    ├── Program.cs                   Configuración: DI, JSON, middleware, Swagger
    ├── Controllers/                 Capa de presentación (endpoints REST)
    ├── Services/                    Lógica de negocio
    │   ├── Interfaces/              Contratos IPacienteService, IMedicoService, ICitaService
    │   └── HorarioAtencion.cs       Reglas del horario de la clínica
    ├── Repositories/                Persistencia simulada en memoria
    │   ├── Interfaces/IRepository.cs
    │   └── InMemoryRepository.cs    List<T> + lock + id autoincremental
    ├── Models/                      Entidades del dominio (EntidadBase, Persona, Paciente, Medico, Cita, enums)
    ├── DTOs/                        Objetos de entrada/salida con validaciones
    ├── Mappers/                     Conversión entidad → DTO
    ├── Validation/                  Atributos personalizados ([FechaFutura], [FechaNacimientoValida])
    ├── Exceptions/                  Excepciones de la API (404, 409, 422)
    ├── Middleware/                  Manejo centralizado de excepciones
    ├── Configuration/               Swagger, formato de errores y fechas
    └── Data/DatosIniciales.cs       Datos de ejemplo al arrancar
```

**Flujo de una petición:**

```
Cliente (Postman) → Controller → Service (interfaz) → Repository (interfaz) → List<T> en memoria
                         ↑ valida DTO                ↑ reglas de negocio
          ExceptionHandlingMiddleware convierte cualquier excepción en una respuesta JSON uniforme
```

| Capa | Responsabilidad | No hace |
| --- | --- | --- |
| **Controllers** | Recibe la petición HTTP, valida el DTO (automático con `[ApiController]`) y devuelve el código de estado | No tiene lógica de negocio ni `try/catch` |
| **Services** | Aplica las reglas de negocio y coordina repositorios | No conoce HTTP |
| **Repositories** | Guarda y consulta datos en memoria | No valida reglas de negocio |
| **Models** | Entidades con encapsulamiento (`private set`) y métodos de dominio (`Cancelar()`, `Reprogramar()`) | No se exponen directamente: la API devuelve DTOs |

**Principios aplicados:** encapsulamiento (propiedades de solo lectura y cambios de estado mediante métodos), abstracción (interfaces de servicios y repositorio), herencia (`Persona` → `Paciente` y `Medico`; `EntidadBase` → todas las entidades), inyección de dependencias y repositorio genérico reutilizable (`InMemoryRepository<T>`).

## Modelo de dominio y reglas de negocio

```
EntidadBase (Id, FechaRegistro, FechaActualizacion)
├── Persona (Nombres, Apellidos, Dni, Email, Telefono)
│   ├── Paciente (FechaNacimiento, Edad)
│   └── Medico (Cmp, Especialidad, Activo)
└── Cita (PacienteId, MedicoId, FechaHora, Motivo, Estado, MotivoCancelacion)
```

**Estados de una cita:** `Programada` → `Cancelada` o `Atendida`. Solo una cita **Programada** puede reprogramarse, cancelarse o marcarse como atendida.

| Regla | Respuesta si se incumple |
| --- | --- |
| La fecha de la cita debe ser futura | 400 |
| Horario de atención: lunes a sábado, de 08:00 a 20:00 | 422 |
| Las citas duran 30 minutos y empiezan en bloques de :00 o :30 | 422 |
| El médico debe estar activo | 422 |
| Un médico no puede tener dos citas que se crucen | 409 |
| Un paciente no puede tener dos citas que se crucen | 409 |
| Una cita cancelada libera su horario | — |
| No se elimina una cita ya atendida (historial clínico) | 422 |
| DNI de paciente único; CMP y DNI de médico únicos | 409 |
| No se elimina un paciente o médico con citas registradas (integridad referencial) | 409 |

## Endpoints

Base: `http://localhost:5080`

### Citas – `/api/citas`

| Método | Ruta | Descripción | Respuestas |
| --- | --- | --- | --- |
| GET | `/api/citas` | Lista citas. Filtros opcionales: `pacienteId`, `medicoId`, `estado`, `fecha` | 200, 404 |
| GET | `/api/citas/{id}` | Obtiene una cita | 200, 404 |
| POST | `/api/citas` | Registra una cita | 201, 400, 404, 409, 422 |
| PUT | `/api/citas/{id}` | Reprograma la cita (médico, fecha y motivo) | 200, 400, 404, 409, 422 |
| PATCH | `/api/citas/{id}/cancelar` | Cancela la cita y libera el horario | 200, 400, 404, 422 |
| PATCH | `/api/citas/{id}/atender` | Marca la cita como atendida | 200, 404, 422 |
| DELETE | `/api/citas/{id}` | Elimina la cita | 204, 404, 422 |

### Pacientes – `/api/pacientes`

| Método | Ruta | Descripción | Respuestas |
| --- | --- | --- | --- |
| GET | `/api/pacientes?busqueda=` | Lista pacientes; busca por nombre o DNI | 200 |
| GET | `/api/pacientes/{id}` | Obtiene un paciente | 200, 404 |
| POST | `/api/pacientes` | Registra un paciente | 201, 400, 409 |
| PUT | `/api/pacientes/{id}` | Actualiza un paciente | 200, 400, 404, 409 |
| DELETE | `/api/pacientes/{id}` | Elimina un paciente sin citas | 204, 404, 409 |
| GET | `/api/pacientes/{id}/citas` | Citas del paciente | 200, 404 |

### Médicos – `/api/medicos`

| Método | Ruta | Descripción | Respuestas |
| --- | --- | --- | --- |
| GET | `/api/medicos?especialidad=` | Lista médicos; filtra por especialidad | 200 |
| GET | `/api/medicos/{id}` | Obtiene un médico | 200, 404 |
| POST | `/api/medicos` | Registra un médico | 201, 400, 409 |
| PUT | `/api/medicos/{id}` | Actualiza un médico (incluye `activo`) | 200, 400, 404, 409 |
| DELETE | `/api/medicos/{id}` | Elimina un médico sin citas | 204, 404, 409 |
| GET | `/api/medicos/{id}/citas?fecha=` | Citas del médico, opcionalmente de un día | 200, 404 |
| GET | `/api/medicos/{id}/disponibilidad?fecha=` | Horarios libres del médico en un día | 200, 400, 404, 422 |

Especialidades: `MedicinaGeneral`, `Pediatria`, `Cardiologia`, `Dermatologia`, `Ginecologia`, `Traumatologia`.

## Formato de respuestas y códigos HTTP

Todas las respuestas exitosas usan el mismo envoltorio:

```json
{
  "exito": true,
  "mensaje": "Cita registrada correctamente.",
  "datos": { }
}
```

Todos los errores, sin excepción (validación, reglas de negocio, rutas inexistentes o errores internos), usan esta estructura:

```json
{
  "exito": false,
  "estado": 409,
  "error": "Conflicto con datos existentes",
  "mensaje": "El médico ya tiene una cita el 02/11/2026 a las 10:00. Consulte GET /api/medicos/4/disponibilidad?fecha=2026-11-02.",
  "errores": null,
  "ruta": "POST /api/citas",
  "fecha": "2026-10-26T19:59:30",
  "traceId": "0HNPJOM73OUE7:00000010"
}
```

| Código | Cuándo se usa |
| --- | --- |
| 200 OK | Consulta, actualización, cancelación o atención exitosa |
| 201 Created | Recurso creado; la cabecera `Location` apunta a él |
| 204 No Content | Recurso eliminado |
| 400 Bad Request | Datos de entrada inválidos (campo `errores` con el detalle por campo) |
| 404 Not Found | El recurso o la ruta no existen |
| 409 Conflict | Duplicados (DNI, CMP), horario ocupado o eliminación con datos relacionados |
| 422 Unprocessable Entity | Datos válidos que incumplen una regla del negocio (domingo, fuera de horario, cita no programada) |
| 500 Internal Server Error | Error inesperado; se registra en el log y no se exponen detalles internos |

Las fechas se envían y se devuelven en **hora local**, con el formato `yyyy-MM-ddTHH:mm:ss` (por ejemplo `2026-11-02T10:00:00`).

## Ejemplos de uso

### Registrar una cita

```http
POST /api/citas
Content-Type: application/json

{
  "pacienteId": 1,
  "medicoId": 1,
  "fechaHora": "2026-11-02T10:00:00",
  "motivo": "Control de presión arterial"
}
```

Respuesta **201 Created** (cabecera `Location: /api/citas/4`):

```json
{
  "exito": true,
  "mensaje": "Cita registrada correctamente.",
  "datos": {
    "id": 4,
    "fechaHora": "2026-11-02T10:00:00",
    "fechaHoraFin": "2026-11-02T10:30:00",
    "motivo": "Control de presión arterial",
    "estado": "Programada",
    "motivoCancelacion": null,
    "paciente": { "id": 1, "nombreCompleto": "Ana Lucía Torres Vega", "dni": "45879632" },
    "medico": { "id": 1, "nombreCompleto": "Jorge Luis Ramírez Soto", "especialidad": "Cardiologia" },
    "fechaRegistro": "2026-10-26T19:59:30",
    "fechaActualizacion": null
  }
}
```

### Consultar horarios libres antes de reservar

```http
GET /api/medicos/1/disponibilidad?fecha=2026-11-02
```

```json
{
  "exito": true,
  "mensaje": "24 horario(s) disponible(s).",
  "datos": {
    "medicoId": 1,
    "medico": "Jorge Luis Ramírez Soto",
    "fecha": "2026-11-02",
    "duracionCitaMinutos": 30,
    "horariosDisponibles": ["08:00", "08:30", "09:00", "…", "19:30"]
  }
}
```

### Cancelar una cita

```http
PATCH /api/citas/4/cancelar
Content-Type: application/json

{ "motivoCancelacion": "El paciente viajará esa semana" }
```

### Error de validación (400)

```json
{
  "exito": false,
  "estado": 400,
  "error": "Datos de entrada inválidos",
  "mensaje": "Revise los campos indicados en 'errores'.",
  "errores": {
    "dni": ["El DNI debe tener exactamente 8 dígitos."],
    "email": ["El correo no tiene un formato válido."],
    "telefono": ["El teléfono debe ser un celular de 9 dígitos que empiece en 9."]
  },
  "ruta": "POST /api/pacientes",
  "fecha": "2026-10-26T19:59:30",
  "traceId": "0HNPJOM73OUE7:00000003"
}
```

### Con curl

```bash
curl http://localhost:5080/api/citas?estado=Programada
curl -X POST http://localhost:5080/api/citas -H "Content-Type: application/json" \
     -d '{"pacienteId":1,"medicoId":1,"fechaHora":"2026-11-02T10:00:00","motivo":"Control de presión arterial"}'
```

## Pruebas con Postman

La carpeta `postman/` contiene:

- `ClinicaCitas.postman_collection.json`: **38 solicitudes con 162 verificaciones automáticas**, organizadas en 4 carpetas (Pacientes, Médicos, Citas, Integridad y errores). Cubren el CRUD completo, las validaciones y todas las reglas de negocio (400, 404, 409 y 422).
- `ClinicaCitas.postman_environment.json`: environment «Clínica – Local» con `baseUrl = http://localhost:5080`.

**Ejecutar en Postman**

1. Inicia la API (`dotnet run --project src/ClinicaCitas.Api`).
2. En Postman: **Import** → selecciona los dos archivos de `postman/`.
3. Elige el environment **Clínica – Local**.
4. Abre la colección y pulsa **Run collection** → **Run**. Ejecútala completa y en orden: cada solicitud guarda los ids que usa la siguiente.

**Ejecutar por consola con Newman**

```bash
npm install -g newman
newman run postman/ClinicaCitas.postman_collection.json -e postman/ClinicaCitas.postman_environment.json
```

Las fechas de prueba se calculan solas (siempre el próximo lunes), así que la colección funciona cualquier día. Puede ejecutarse varias veces sin reiniciar la API: los DNI y CMP de prueba se generan al azar.

**Resultado:** 38 de 38 solicitudes y 162 de 162 pruebas aprobadas.

![Resumen de la ejecución](docs/capturas/01_resumen.png)

| Escenario | Captura |
| --- | --- |
| Registrar cita (201) | [05_post_cita_201.png](docs/capturas/05_post_cita_201.png) |
| Validación de datos (400) | [03_validacion_400.png](docs/capturas/03_validacion_400.png) |
| Horario ocupado (409) | [06_conflicto_409.png](docs/capturas/06_conflicto_409.png) |
| Cita en domingo (422) | [07_domingo_422.png](docs/capturas/07_domingo_422.png) |
| Reprogramar (PUT) | [09_put_reprogramar.png](docs/capturas/09_put_reprogramar.png) |
| Cancelar (PATCH) | [10_patch_cancelar.png](docs/capturas/10_patch_cancelar.png) |
| Eliminar (204) | [12_delete_204.png](docs/capturas/12_delete_204.png) |

El reporte completo de Newman, con cada solicitud y respuesta, está en `docs/pruebas/reporte-newman.html`; se abre en el navegador y necesita internet para cargar los estilos.

## Equipo y roles

| Integrante | Rol | Responsabilidades |
| --- | --- | --- |
| Leonardo Jaime Sánchez Ramírez | Líder técnico y arquitectura | Estructura en capas, repositorio genérico en memoria, inyección de dependencias, middleware de errores |
| [Integrante 2] | Dominio y lógica de negocio | Modelos (entidades, herencia, estados), servicios y reglas de citas |
| [Integrante 3] | API, documentación y pruebas | Controladores, DTOs y validaciones, Swagger, colección de Postman y README |
