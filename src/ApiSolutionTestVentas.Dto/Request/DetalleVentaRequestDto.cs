using ApiSolutionTestVentas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiSolutionTestVentas.Dto.Request;

public class DetalleVentaRequestDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; } = 0;
}
