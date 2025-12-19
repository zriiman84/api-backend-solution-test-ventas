using System.Globalization;
using System.Linq.Expressions;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Entities.Info;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly ILogger<VentaService> _logger;
    private readonly IMapper _mapper;

    public VentaService
    (
        IVentaRepository ventaRepository,
        IClienteRepository clienteRepository,
        IEmpleadoRepository empleadoRepository,
        IProductoRepository productoRepository,
        ILogger<VentaService> logger,
        IMapper mapper)
    {
        this._ventaRepository = ventaRepository;
        this._clienteRepository = clienteRepository;
        this._empleadoRepository = empleadoRepository;
        this._productoRepository = productoRepository;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<BaseResponseGeneric<VentaResponseDto>> GetAsync(int idVenta)
    {
        var response = new BaseResponseGeneric<VentaResponseDto>();

        try
        {
            var ventaDb = await _ventaRepository.GetVentaByIdAsync(idVenta);

            if (ventaDb is null)
            {
                response.Message = $"No se encontró la Venta con id {idVenta}..";
                _logger.LogWarning(response.Message);
                return response;
            }

            var ventaResponseDto = _mapper.Map<VentaResponseDto>(ventaDb);
            var detalleVentaDb = await _ventaRepository.GetDetalleVentaByIdVentaAsync(idVenta);
            ventaResponseDto.DetalleVenta = _mapper.Map<ICollection<DetalleVentaInfo>>(detalleVentaDb).ToList();

            response.Data = ventaResponseDto;
            response.Success = true;
            response.Message = $"Se obtuvo correctamente la Venta con id {idVenta}";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el Venta con id {idVenta}";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
    
    public async Task<BaseResponseGeneric<ICollection<VentaResponseDto>>> GetAsync
    (bool flagPagination, PaginationDto? pagination)
    {
        var response = new BaseResponseGeneric<ICollection<VentaResponseDto>>();

        try
        {
            ICollection<Venta> listaVentasDb;
            
            // 1. Obtener las ventas (con paginación o sin paginación)
            if (flagPagination && pagination is not null)
            {
                listaVentasDb = await _ventaRepository.GetAsync(pagination);
            }
            else
            {
                listaVentasDb = await _ventaRepository.GetAsync();
            }
            
            var listaVentaResponseDto = _mapper.Map<ICollection<VentaResponseDto>>(listaVentasDb);
            response.Success = true;

            if (listaVentaResponseDto is null || listaVentaResponseDto.Count == 0)
            {
                response.Message = "No se encontraron elementos para la búsqueda.";
                _logger.LogWarning(response.Message);
                return response;
            }

            // 2. Asociar los DetalleVenta (DetalleVentaInfo) a cada Venta
            var listaVentaResponseDtoFinal = await AsociarDetalleVentaPorVenta(listaVentaResponseDto);
            response.Data = listaVentaResponseDtoFinal;
            response.Message =
                $"Se obtuvo correctamente la lista de ventas con {listaVentaResponseDtoFinal.Count} ventas asociadas...";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la búsqueda de las ventas.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
    
    public async Task<BaseResponseGeneric<ICollection<VentaResponseDto>>> GetAsync(SearchVentaDto? searchVentaDto,
        PaginationDto pagination)
    {
        var response = new BaseResponseGeneric<ICollection<VentaResponseDto>>();

        try
        {
            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;

            // 1. Definir el Predicado (filtro WHERE)
            // Como no estamos considerando horas, se agrega 1 día a la fecha final para incluir todas las ventas del día anterior.
            if (searchVentaDto is not null)
            {
                fechaInicio = !String.IsNullOrEmpty(searchVentaDto.FechaInicio)
                    ? DateTime.ParseExact(searchVentaDto.FechaInicio, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture)
                    : null;
                fechaFin = !String.IsNullOrEmpty(searchVentaDto.FechaFin)
                    ? DateTime.ParseExact(searchVentaDto.FechaFin, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture).AddDays(1)
                    : null;
            }

            Expression<Func<Venta, bool>> filtro = u =>
                (fechaInicio == null || u.FechaHoraVenta >= fechaInicio)
                && (fechaFin == null || u.FechaHoraVenta < fechaFin);

            // 2. Definir el Ordenamiento (ORDER BY): Ordenar por Id
            Expression<Func<Venta, int>> orden = u => u.Id;

            // 3. Llamada al método asíncrono
            var listaVentasDb = await _ventaRepository.GetAsync(
                predicate: filtro,
                orderBy: orden,
                pagination);

            var listaVentaResponseDto = _mapper.Map<ICollection<VentaResponseDto>>(listaVentasDb);
            response.Success = true;

            if (listaVentaResponseDto is null || listaVentaResponseDto.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
                return response;
            }

            // 4. Asociar los DetalleVenta a cada Venta
            var listaVentaResponseDtoFinal = await AsociarDetalleVentaPorVenta(listaVentaResponseDto);
            response.Data = listaVentaResponseDtoFinal;
            response.Message =
                $"Se obtuvo correctamente la lista de ventas con {listaVentaResponseDtoFinal.Count} elementos...";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la búsqueda de las ventas.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
    
    public async Task<BaseResponseGeneric<ICollection<VentaResponseDto>>> GetAsync
    (string emailCliente,
        string? nombresEmpleado,
        string? apellidosEmpleado,
        PaginationDto pagination)
    {
        var response = new BaseResponseGeneric<ICollection<VentaResponseDto>>();

        try
        {
            // 1. Definir el Predicado (filtro WHERE) 
            Expression<Func<Venta, bool>> filtro;

            // 1.1 Enviando sólo cliente
            if(string.IsNullOrEmpty(nombresEmpleado) && string.IsNullOrEmpty(apellidosEmpleado))
            {
                filtro = u => (u.Cliente.Email == emailCliente);
            }else
            {
                filtro = u =>
                (u.Cliente.Email == emailCliente)
                &&
                (u.Empleado != null &&
                     // Y debe cumplir con los filtros de nombre y apellido
                     u.Empleado.Nombre.Contains(!String.IsNullOrEmpty(nombresEmpleado)
                         ? nombresEmpleado
                         : string.Empty) &&
                     u.Empleado.Apellidos.Contains(!String.IsNullOrEmpty(apellidosEmpleado)
                         ? apellidosEmpleado
                         : string.Empty));
            }

             // 2. Definir el Ordenamiento (ORDER BY): Ordenar por Id
             Expression<Func<Venta, int>> orden = u => u.Id;

            // 3. Llamada al método asíncrono
            var listaVentasDb = await _ventaRepository.GetAsync(
                predicate: filtro,
                orderBy: orden,
                pagination);

            var listaVentaResponseDto = _mapper.Map<ICollection<VentaResponseDto>>(listaVentasDb);
            response.Success = true;

            if (listaVentaResponseDto is null || listaVentaResponseDto.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
                return response;
            }

            // 4. Asociar los DetalleVenta (DetalleVentaInfo) a cada Venta
            var listaVentaResponseDtoFinal = await AsociarDetalleVentaPorVenta(listaVentaResponseDto);
            response.Data = listaVentaResponseDtoFinal;
            response.Message =
                $"Se obtuvo correctamente la lista de ventas con {listaVentaResponseDtoFinal.Count} ventas asociadas...";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la búsqueda de las ventas.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<int>> RealizarVentaAsync(string email, VentaRequestDto ventaRequestDto)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            //Inicia la transacción
            await _ventaRepository.CrearTransaccionAsync();

            //1. Definiendo Cabecera
            Venta ventaDb = new Venta();

            //2. Validando cliente
            var clienteDb = await _clienteRepository.GetAsync(email);
            if (clienteDb is null)
            {
                response.Message =
                    $"No se puede realizar la venta. El cliente con email {email} no se encuentra registrado.";
                _logger.LogWarning(response.Message);
                return response;
            }

            ventaDb.ClienteId = clienteDb.Id;

            //3. Validando empleado (el empleado no es obligatorio en la venta)
            if (ventaRequestDto.EmpleadoId is not null && ventaRequestDto.EmpleadoId > 0)
            {
                var empleadoDb = await _empleadoRepository.GetAsync(ventaRequestDto.EmpleadoId ?? 0);
                if (empleadoDb is null)
                {
                    response.Message =
                        $"No se puede realizar la venta. El empleado con id {ventaRequestDto.EmpleadoId} no fue encontrado.";
                    _logger.LogWarning(response.Message);
                    return response;
                }

                ventaDb.EmpleadoId = empleadoDb.Id;
            }

            var listaDetalleVenta = ventaRequestDto.DetalleVenta;

            //4.Validar si existen elementos detalle
            if (listaDetalleVenta is null || listaDetalleVenta.Count == 0)
            {
                response.Message = $"No se tiene detalle de productos a registrar...";
                _logger.LogWarning(response.Message);
                return response;
            }

            //5. Completando los datos de la Cabecera de la Venta
            var listaProductosDb = await ObtenerListaProductos(listaDetalleVenta);

            ventaDb.CantidadTotalArticulos = listaDetalleVenta.Sum(p => p.Cantidad);
            ventaDb.MontoTotalVenta = listaProductosDb.Sum(x =>
            {
                // Buscar la cantidad requerida para este producto específico (el riesgo se aísla aquí)
                var itemVenta = listaDetalleVenta.FirstOrDefault(d => d.ProductoId == x.Id);
                if (itemVenta == null) return 0;
                return x.PrecioUnitario * itemVenta.Cantidad;
            });

            //6. Registrar la cabecera de la venta
            var idVenta = await _ventaRepository.AddCabeceraVentaAsync(ventaDb);

            if (idVenta == 0)
            {
                throw new Exception($"La Cabecera de la Venta no pudo ser registrada correctamente...");
            }

            //7. Definiendo Detalle de la Venta : Se puede hacer más óptimo con Diccionarios
            var listaVentaProducto = listaDetalleVenta.Select(p =>
            {
                // 7.1. Consulto el producto
                var productoDb = listaProductosDb.FirstOrDefault(x => x.Id == p.ProductoId);

                // 7.2. Obtener el precio unitario
                decimal precioUnitarioSeguro = productoDb?.PrecioUnitario ?? 0;

                // 7.3. Crear el nuevo objeto VentaProducto
                return new VentaProducto
                {
                    ProductoId = p.ProductoId,
                    VentaId = idVenta,
                    SubTotal = precioUnitarioSeguro * p.Cantidad,
                    Cantidad = p.Cantidad,
                    PrecioCompra = precioUnitarioSeguro,
                };
            }).ToList();

            //8. Registrar el detalle de la venta
            var itemsAfectados = await _ventaRepository.AddDetalleVentaAsync(listaVentaProducto);

            if (itemsAfectados == 0)  throw new Exception($"El detalle de la Venta no pudo ser registrado correctamente...");

            //9. Actualizar el stock de los productos
            var listaProductosAActualizarStock =
                await _productoRepository.GetListProductsByListIdAsync(
                    listaVentaProducto.Select(item => item.ProductoId).ToList(), false);
            
            foreach (var item in listaProductosAActualizarStock)
            {
                var itemVentaProducto = listaVentaProducto.FirstOrDefault(x => x.ProductoId == item.Id);
                item.Stock = item.Stock - (itemVentaProducto?.Cantidad ?? 0);
            }
            
            var itemsAfectado = await _productoRepository.UpdateAsync();
            if (itemsAfectado == 0)  throw new Exception($"El stock no se pudo actualizar correctamente...");

            //10. Confirmar los cambios - Finaliza la Transacción
            await _ventaRepository.FinalizarTransaccionAsync();

            response.Data = idVenta;
            response.Success = true;
            response.Message = $"Venta realizada correctamente con id {idVenta} para el cliente con email {email}";
            _logger.LogInformation(response.Message);
        }

        catch (Exception ex)
        {
            await _ventaRepository.RollbackTransaccionAsync();
            response.Success = false;
            response.ErrorMessage = "Error al registrar la Venta.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    private async Task<ICollection<Producto>> ObtenerListaProductos(List<DetalleVenta> listaDetalleVenta)
    {
        // 1. Obtener todos los IDs
        var listaIdsProductos = listaDetalleVenta.Select(item => item.ProductoId).ToList();

        // 2. Obtener todos los productos  existentes en UNA SOLA consulta a la DB
        var listaProductosExistentes = await _productoRepository.GetListProductsByListIdAsync(listaIdsProductos, true);

        // 3. Identificar los IDs inexistentes (Comparación en memoria)
        var listaIdsInexistentes = listaIdsProductos
            .Except(listaProductosExistentes.Select(p => p.Id)) // Busca IDs en la lista original que NO están en la DB
            .ToList();

        // 4.Lanzar excepción de existir productos no existentes
        if (listaIdsInexistentes.Any())
        {
            // Lanza una sola excepción con todos los productos faltantes
            string idsFaltantes = string.Join(", ", listaIdsInexistentes);
            throw new InvalidOperationException(
                $"Los productos relacionados a la siguiente lista de Id's no existen: {idsFaltantes}");
        }

        return listaProductosExistentes;
    }

    private async Task<ICollection<VentaProducto>> ObtenerDetalleVentas(
        ICollection<VentaResponseDto> listaVentaResponseDto)
    {
        // 1. Obtener todos los IDs
        var listaIdsVentas = listaVentaResponseDto.Select(item => item.IdVenta).ToList();

        // 2. Obtener todos los productos existentes en UNA SOLA consulta a la DB
        var listaVentaProducto = await _ventaRepository.GetDetalleVentaByListIdVentaAsync(listaIdsVentas);

        return listaVentaProducto;
    }

    private async Task<ICollection<VentaResponseDto>> AsociarDetalleVentaPorVenta(
        ICollection<VentaResponseDto> listaVentaResponseDto)
    {
        var listaVentaProducto = await ObtenerDetalleVentas(listaVentaResponseDto);

        //creo un diccionario (forma eficiente)
        var detallePorVentaDictionary = listaVentaProducto
            .GroupBy(vp => vp.VentaId)
            .ToDictionary(g => g.Key,
                g => g.ToList()); // Key: IdVenta, Value: List<VentaProducto>

        var listaVentaResponseDtoFinal = listaVentaResponseDto.Select(ventaDto =>
        {
            // Intentar obtener el detalle del diccionario usando el IdVenta del DTO.
            if (detallePorVentaDictionary.TryGetValue(ventaDto.IdVenta, out var detalleProductos))
            {
                ventaDto.DetalleVenta = _mapper.Map<ICollection<DetalleVentaInfo>>(detalleProductos).ToList();
            }

            // Retornar el objeto DTO modificado.
            return ventaDto;
        }).ToList();

        //FORMA MENOS EFICIENTE: O(N) X O(M)
        /*
        foreach (var venta in listaVentaResponseDto)
        {
            var detallesVentaEspecifica = listaVentaProducto.Where(x => x.VentaId == venta.IdVenta);
            venta.DetalleVenta = _mapper.Map<ICollection<DetalleVentaInfo>>(detallesVentaEspecifica).ToList();
        }
        */

        return listaVentaResponseDtoFinal;
    }

    //Será invocado por el MinimalApi de Reportes por Cliente
    public async Task<BaseResponseGeneric<ICollection<VentaReporteClienteResponseDto>>> GetAsyncSaleReportByClient(
        string? fechaInicio, string? fechaFin)
    {
        var response = new BaseResponseGeneric<ICollection<VentaReporteClienteResponseDto>>();

        try
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            startDate = !String.IsNullOrEmpty(fechaInicio)
                ? DateTime.ParseExact(fechaInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null;
            endDate = !String.IsNullOrEmpty(fechaFin)
                ? DateTime.ParseExact(fechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1)
                : null;

            var reporteVentaInfoDb = await _ventaRepository.GetVentaReporteCliente(startDate, endDate);
            response.Success = true;

            if (reporteVentaInfoDb is null || reporteVentaInfoDb.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
                return response;
            }

            var ventaReporteClienteResponseDto =
                _mapper.Map<ICollection<VentaReporteClienteResponseDto>>(reporteVentaInfoDb);

            response.Data = ventaReporteClienteResponseDto;
            response.Message = $"Se obtuvo correctamente el reporte de ventas por cliente con {ventaReporteClienteResponseDto.Count} ventas asociadas.";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el reporte de Ventas por Cliente";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    //Será invocado por el MinimalApi de Reportes por Producto
    public async Task<BaseResponseGeneric<ICollection<VentaReporteProductoResponseDto>>> GetAsyncSaleReportByProduct(
        string? fechaInicio, string? fechaFin)
    {
        var response = new BaseResponseGeneric<ICollection<VentaReporteProductoResponseDto>>();

        try
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            startDate = !String.IsNullOrEmpty(fechaInicio)
                ? DateTime.ParseExact(fechaInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null;
            endDate = !String.IsNullOrEmpty(fechaFin)
                ? DateTime.ParseExact(fechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1)
                : null;

            var reporteVentaInfoDb = await _ventaRepository.GetVentaReporteProducto(startDate, endDate);
            response.Success = true;

            if(reporteVentaInfoDb == null || reporteVentaInfoDb.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
                return response;

            }

            var ventaReporteProductoResponseDto =
                _mapper.Map<ICollection<VentaReporteProductoResponseDto>>(reporteVentaInfoDb);

            response.Data = ventaReporteProductoResponseDto;
            
            response.Message = $"Se obtuvo el reporte de ventas por productos con {ventaReporteProductoResponseDto.Count} ventas asociadas.";
            _logger.LogInformation(response.Message);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"Error al obtener el reporte de Ventas por producto.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<ProductoResponseDto>>> GetListProductsByListIdAsync(List<DetalleVentaRequestDto> detallesVentaRequestDto)
    {
        var response = new BaseResponseGeneric<ICollection<ProductoResponseDto>>();

        try
        {
            if (detallesVentaRequestDto is null || detallesVentaRequestDto.Count == 0)
            {
                response.Message = $"No se han enviado detalle de productos.";
                _logger.LogWarning(response.Message);
                return response;

            }

            // 1. Obtener todos los IDs
            var listaIdsProductos = detallesVentaRequestDto.Select(item => item.ProductoId).ToList();

            // 2. Obtener todos los productos  existentes en UNA SOLA consulta a la DB
            var listaProductosExistentes = await _productoRepository.GetListProductsByListIdAsync(listaIdsProductos, true);

            // 3. Identificar los IDs inexistentes (Comparación en memoria)
            var listaIdsInexistentes = listaIdsProductos
                .Except(listaProductosExistentes.Select(p => p.Id)) // Busca IDs en la lista original que NO están en la DB
                .ToList();

            // 4.Lanzar excepción de existir productos no existentes
            if (listaIdsInexistentes.Any())
            {
                string idsFaltantes = string.Join(", ", listaIdsInexistentes);
                response.Success = false;
                response.Message = $"Los productos relacionados a la siguiente lista de Id's no existen: {idsFaltantes}";
                _logger.LogWarning(response.Message);
                return response;
            }

            response.Data = _mapper.Map<ICollection<ProductoResponseDto>>(listaProductosExistentes);
            response.Success = true;

            if (response.Data is null || response.Data.Count == 0)
            {
                response.Message = "No se encontraron coincidencias para la búsqueda realizada.";
                _logger.LogWarning(response.Message);
            }
            else
            {
                response.Message =
                    $"Se obtuvo correctamente la lista de productos con {response.Data.Count} elementos...";
                _logger.LogInformation(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener la lista de productos.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
}