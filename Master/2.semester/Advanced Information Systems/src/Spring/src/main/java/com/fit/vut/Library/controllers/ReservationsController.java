package com.fit.vut.Library.controllers;

import com.fit.vut.Library.dtos.ReservationDto;
import com.fit.vut.Library.services.ReservationService;
import com.fit.vut.Library.services.filters.ReservationFilter;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.server.ResponseStatusException;

import java.util.List;

@RestController
@RequestMapping("/reservations")
@Tag(name="Reservations")
@ApiResponses(value = {
    @ApiResponse(responseCode = "200", description = "Successful operation"),
    @ApiResponse(responseCode = "400", description = "Bad request"),
    @ApiResponse(responseCode = "500", description = "Internal server error")
})
public class ReservationsController {

    private final ReservationService service;

    public ReservationsController(ReservationService service) {
        this.service = service;
    }

    @Operation(summary = "Gets reservation with given id.")
    @GetMapping(value = "/{id}", produces = {"application/json"})
    @ApiResponse(responseCode = "404", description = "Reservation not found")
    public ReservationDto getById(@PathVariable Long id) {
        ReservationDto reservation = service.getById(id);
        if (reservation == null)
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Reservation with id " + id + " not found.");

        return reservation;
    }

    @Operation(summary = "Gets all reservations meeting filter requirements.")
    @GetMapping(value = "/", produces = {"application/json"})
    public List<ReservationDto> get(ReservationFilter filter) {
        return service.get(filter);
    }

    @Operation(summary = "Creates new reservation.")
    @PostMapping()
    public void create(@RequestBody ReservationDto reservation) {
        if (reservation.getId() != 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be 0 when creating new reservation.");

        service.save(reservation);
    }

    @Operation(summary = "Updates reservation.")
    @PutMapping()
    public void update(@RequestBody ReservationDto reservation) {

        if (reservation.getId() <= 0)
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Id must be greater than 0 when updating a reservation.");

        service.save(reservation);
    }

    @Operation(summary = "Deletes a reservation.")
    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) { service.delete(id); }
}
