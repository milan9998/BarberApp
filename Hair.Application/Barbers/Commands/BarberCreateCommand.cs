﻿using System.Globalization;
using System.Text.RegularExpressions;
using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Interfaces;
using Hair.Application.Common.Mappers;
using Hair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Hair.Application.Barbers.Commands;

public record BarberCreateCommand(BarberCreateDto Barber) : IRequest<BarberResponseDto?>;

public class BarberCreateCommandHandler(IBarberService barberService) : IRequestHandler<BarberCreateCommand, BarberResponseDto?>
{
    public async Task<BarberResponseDto?> Handle(BarberCreateCommand request, CancellationToken cancellationToken)
    {
        var x = await barberService.BarberCreateAsync(request.Barber, cancellationToken);
        return x;
    }
    
}