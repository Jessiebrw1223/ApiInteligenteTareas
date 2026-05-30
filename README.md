# API Inteligente de Tareas y Análisis

Proyecto desarrollado para la Evaluación Continua N.° 3.

**Stack:** ASP.NET Core Web API (.NET 8), EF Core, SQLite, ML.NET y consumo de API externa.

## Funcionalidades implementadas

- API RESTful para gestionar tareas internas.
- EF Core con SQLite.
- CRUD completo de tareas.
- Filtros por estado, prioridad y rango de fechas.
- Consumo de API externa `https://jsonplaceholder.typicode.com/todos`.
- Endpoint ML.NET para análisis de sentimiento.
- Swagger habilitado.
- Dockerfile preparado para Render.
- Workflow de GitHub Actions para validar compilación.

## Estructura del proyecto

```text
ApiInteligenteTareas
├── Controllers
│   ├── TareasController.cs
│   ├── TareasExternasController.cs
│   └── MlController.cs
├── Data
│   └── AppDbContext.cs
├── DTOs
├── MLModels
├── Models
├── Services
├── Program.cs
├── Dockerfile
├── render.yaml
└── README.md
```

## Ejecución local

### 1. Restaurar paquetes

```bash
dotnet restore
```

### 2. Ejecutar el proyecto

```bash
dotnet run
```

### 3. Abrir Swagger

```text
http://localhost:5048/swagger
```

> Nota: El proyecto usa `Database.EnsureCreated()` para crear automáticamente la base SQLite al iniciar. Esto facilita la revisión rápida en evaluación.

## Comandos de migración EF Core

Si deseas usar migraciones formales, ejecuta:

```bash
dotnet tool install --global dotnet-ef
```

```bash
dotnet ef migrations add InitialCreate
```

```bash
dotnet ef database update
```

## Endpoints implementados

### Tareas internas

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/tareas` | Lista tareas internas |
| GET | `/api/tareas/{id}` | Obtiene tarea por ID |
| POST | `/api/tareas` | Crea una tarea |
| PUT | `/api/tareas/{id}` | Actualiza una tarea |
| DELETE | `/api/tareas/{id}` | Elimina una tarea |

### Filtros

```http
GET /api/tareas?estado=Pendiente
GET /api/tareas?prioridad=Alta
GET /api/tareas?fechaInicio=2026-05-01&fechaFin=2026-05-31
GET /api/tareas?estado=Pendiente&prioridad=Alta&fechaInicio=2026-05-01&fechaFin=2026-05-31
```

### API externa

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/tareas-externas` | Lista tareas desde JSONPlaceholder |
| GET | `/api/tareas-externas/{id}` | Obtiene una tarea externa por ID |

La API externa original devuelve más campos, pero este proyecto mapea la respuesta a un DTO propio:

```json
{
  "externalId": 1,
  "titulo": "delectus aut autem",
  "completado": false
}
```

### ML.NET

| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/ml/sentimiento` | Clasifica un comentario como Positivo o Negativo |

Ejemplo request:

```json
{
  "comentario": "La tarea fue completada correctamente y el sistema funciona bien"
}
```

Ejemplo response:

```json
{
  "comentario": "La tarea fue completada correctamente y el sistema funciona bien",
  "sentimiento": "Positivo"
}
```

## Validaciones implementadas

### Tareas

- El título es obligatorio.
- La prioridad es obligatoria.
- El estado es obligatorio.
- La fecha de vencimiento no puede ser menor a la fecha actual.
- Estados permitidos: `Pendiente`, `EnProceso`, `Completada`.
- Prioridades permitidas: `Baja`, `Media`, `Alta`.

### Filtros

- Si `fechaInicio` es mayor que `fechaFin`, devuelve `400 BadRequest`.
- Si el estado no es válido, devuelve `400 BadRequest`.
- Si la prioridad no es válida, devuelve `400 BadRequest`.

### API externa

- Si la API externa no responde, devuelve error controlado `503 ServiceUnavailable`.
- Si el ID no existe, devuelve `404 NotFound`.
- No se devuelve el JSON original; se usa DTO propio.

## Dataset ML.NET usado

El dataset está definido dentro de `Services/SentimientoService.cs`.

Ejemplos positivos:

- "La tarea fue completada correctamente"
- "El sistema funciona bien"
- "Excelente trabajo del equipo"
- "La implementación fue exitosa"

Ejemplos negativos:

- "La tarea falló y generó errores"
- "El sistema no funciona"
- "Hubo un problema grave en producción"
- "El servicio está caído"

El modelo usa:

- `FeaturizeText` para transformar texto en características numéricas.
- `SdcaLogisticRegression` para clasificación binaria.
- `true` = Positivo.
- `false` = Negativo.

## Ramas recomendadas para GitHub

```bash
git checkout -b feature/api-tareas
# desarrollar pregunta 1
git add .
git commit -m "feat: implementar crud de tareas"
git push origin feature/api-tareas
```

```bash
git checkout main
git pull
git checkout -b feature/filtros-tareas
# desarrollar pregunta 2
git add .
git commit -m "feat: agregar filtros de tareas"
git push origin feature/filtros-tareas
```

```bash
git checkout main
git pull
git checkout -b feature/api-externa-todos
# desarrollar pregunta 3
git add .
git commit -m "feat: consumir api externa de todos"
git push origin feature/api-externa-todos
```

```bash
git checkout main
git pull
git checkout -b feature/mlnet-basico
# desarrollar pregunta 4
git add .
git commit -m "feat: agregar analisis de sentimiento con mlnet"
git push origin feature/mlnet-basico
```

Luego crear Pull Requests desde GitHub hacia `main`.

## Publicación en Render

Este proyecto incluye `Dockerfile` y `render.yaml`.

Pasos:

1. Crear un repositorio en GitHub.
2. Subir este proyecto al repositorio.
3. Entrar a Render.
4. Crear un nuevo **Web Service**.
5. Conectar el repositorio de GitHub.
6. Seleccionar la rama `main`.
7. En runtime/language seleccionar **Docker**.
8. Render detectará el `Dockerfile`.
9. Health Check Path: `/health`.
10. Deploy.

URL esperada después del despliegue:

```text
https://nombre-del-servicio.onrender.com/swagger
```

## Nota sobre SQLite en Render

SQLite funcionará para demostrar la evaluación. Sin embargo, en Render el filesystem puede ser efímero según configuración del servicio. Para producción real se recomienda PostgreSQL.
