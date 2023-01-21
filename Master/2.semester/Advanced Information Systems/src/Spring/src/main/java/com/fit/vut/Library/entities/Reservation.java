package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.ReservationDto;
import com.fit.vut.Library.enums.ReservationEnum;

import javax.persistence.*;
import java.util.Date;

@Entity
@Table(name = "reservation")
public class Reservation extends EntityBase{

    @Column(name = "dateFrom", nullable = false)
    private Date dateFrom;

    @Column(name = "dateUntil", nullable = false)
    private Date dateUntil;

    @Column(name = "state", nullable = false)
    @Enumerated(EnumType.STRING)
    private ReservationEnum state;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="readerId", nullable = false)
    private Reader reader;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="hardCopyExemplarId", nullable = false)
    private HardCopyExemplar hardCopyExemplar;

    public Reservation() { }

    public Reservation(Long id) {
        super(id);
    }

    public Reservation(Long id, Date dateFrom, Date dateUntil, ReservationEnum state,
                       Reader reader, HardCopyExemplar exemplar) {
        super(id);
        this.dateFrom = dateFrom;
        this.dateUntil = dateUntil;
        this.state = state;
        this.reader = reader;
        this.hardCopyExemplar = exemplar;
    }

    public Reservation(Date dateFrom, Date dateUntil, ReservationEnum state, Reader reader, HardCopyExemplar exemplar) {
        this.dateFrom = dateFrom;
        this.dateUntil = dateUntil;
        this.state = state;
        this.reader = reader;
        this.hardCopyExemplar = exemplar;
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

    public Reader getReader() {
        return reader;
    }

    public void setReader(Reader reader) {
        this.reader = reader;
    }

    public HardCopyExemplar getHardCopyExemplar() {
        return hardCopyExemplar;
    }

    public void setHardCopyExemplar(HardCopyExemplar hardCopyExemplar) {
        this.hardCopyExemplar = hardCopyExemplar;
    }

    public ReservationDto toDto() { return new ReservationDto(id, dateFrom, dateUntil, state, reader.toLightDto(), hardCopyExemplar.toLightDto()); }
}
