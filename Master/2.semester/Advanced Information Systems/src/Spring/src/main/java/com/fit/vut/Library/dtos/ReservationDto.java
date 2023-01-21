package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.Reservation;
import com.fit.vut.Library.enums.ReservationEnum;

import java.util.Date;

public class ReservationDto extends DtoBase {

    private Date dateFrom;

    private Date dateUntil;

    private ReservationEnum state;

    private ReaderLightDto reader;

    private ExemplarLightDto exemplar;

    public ReservationDto() {}

    public ReservationDto(Long id, Date dateFrom, Date dateUntil, ReservationEnum state, ReaderLightDto reader, ExemplarLightDto exemplar) {
        super(id);
        this.dateFrom = dateFrom;
        this.dateUntil = dateUntil;
        this.state = state;
        this.reader = reader;
        this.exemplar = exemplar;
    }


    public Date getDateFrom() {
        return dateFrom;
    }

    public void setDateFrom(Date dateFrom) {
        this.dateFrom = dateFrom;
    }

    public Date getDateUntil() {
        return dateUntil;
    }

    public void setDateUntil(Date dateUntil) {
        this.dateUntil = dateUntil;
    }

    public ReservationEnum getState() {
        return state;
    }

    public void setState(ReservationEnum state) {
        this.state = state;
    }

    public ReaderLightDto getReader() {
        return reader;
    }

    public void setReader(ReaderLightDto reader) {
        this.reader = reader;
    }

    public ExemplarLightDto getExemplar() {
        return exemplar;
    }

    public void setExemplar(ExemplarLightDto exemplar) {
        this.exemplar = exemplar;
    }

    public Reservation toEntity() { return new Reservation(id, dateFrom, dateUntil, state, reader.toEntity(), exemplar.toHardCopyExemplarEntity());}
}
