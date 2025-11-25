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
// Usuario NO se puede cambiar, aunque venga en el DTO.
public record EditEmpleadoDto(
    int IdEmpleado,
    string Usuario,  // ignorado para actualizaciones, solo para lectura
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
public record EmpleadoDetailsDto(
    int IdEmpleado,
    string Usuario,
    int IdRol,
    string RolNombre,
    int? IdEspecialidad,
    string? EspecialidadNombre,
    string PersonaNombre,
    string? Correo,
    string? DUI,
    string? Telefono,
    string? Direccion,
    bool Activo
);
