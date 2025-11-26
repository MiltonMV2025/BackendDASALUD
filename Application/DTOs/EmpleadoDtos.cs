namespace Application.DTOs;

// =============================================
// LISTADO → PARA TABLAS / GRIDS
// =============================================
public record EmpleadoRowDto(
    int IdEmpleado,
    string Usuario,
    string FullName,
    string DUI,
    string Rol,
    string Especialidad,
    bool Activo
);

// =============================================
// CREAR EMPLEADO → CREA PERSONA + EMPLEADO
// =============================================
//
// Reglas:
// - Usuario puede venir vacío → backend genera uno
// - Password puede ser null → backend genera una
// - IdEspecialidad puede ser null (por si hay roles sin especialidad)
public record CreateEmpleadoDto(
    string Usuario,
    string? Password,
    string Nombres,
    string Apellidos,
    string? Correo,
    string? DUI,
    string? Telefono,
    string? Direccion,
    int IdRol,
    int? IdEspecialidad,
    bool Activo
);

// =============================================
// EDITAR EMPLEADO → EDITA PERSONA + EMPLEADO
// =============================================
//
// Ahora SÍ se puede cambiar Usuario.
// Password es opcional: si viene null o vacío, se deja la actual.
public record EditEmpleadoDto(
    int IdEmpleado,
    string Usuario,      // se actualiza
    string? Password,    // NUEVO: para cambiar contraseña (opcional)
    string Nombres,
    string Apellidos,
    string? Correo,
    string? DUI,
    string? Telefono,
    string? Direccion,
    int IdRol,
    int? IdEspecialidad,
    bool Activo
);

// =============================================
// DETALLES → PARA FORMULARIOS DE EDICIÓN
// =============================================
//
// Separamos Nombres y Apellidos para llenar el formulario.
public record EmpleadoDetailsDto(
    int IdEmpleado,
    string Usuario,
    int IdRol,
    string RolNombre,
    int? IdEspecialidad,
    string? EspecialidadNombre,
    string Nombres,
    string Apellidos,
    string? Correo,
    string? DUI,
    string? Telefono,
    string? Direccion,
    bool Activo
);