package com.fit.vut.Library.entities;

import com.fit.vut.Library.dtos.FineDto;
import com.fit.vut.Library.enums.FineEnum;

import javax.persistence.*;

@Entity
@Table(name = "fine")
public class Fine extends EntityBase{

    @Column(name = "amount", nullable = false)
    private int amount;

    @Column(name = "state", nullable = false)
    @Enumerated(EnumType.STRING)
    private FineEnum state;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="hardCopyBorrowingId")
    private HardCopyBorrowing hardCopyBorrowing;

    public Fine() {}

    public Fine(Long id) {
        this.id = id;
    }

    public Fine(Long id, int amount, FineEnum state, HardCopyBorrowing borrowing) {
        super(id);
        this.amount = amount;
        this.state = state;
        this.hardCopyBorrowing = borrowing;
    }

    public Fine(int amount, FineEnum state, HardCopyBorrowing borrowing) {
        this.amount = amount;
        this.state = state;
        this.hardCopyBorrowing = borrowing;
    }


    public int getAmount() {
        return amount;
    }

    public void setAmount(int amount) {
        this.amount = amount;
    }

    public FineEnum getState() {
        return state;
    }

    public void setState(FineEnum state) {
        this.state = state;
    }

    public HardCopyBorrowing getHardCopyBorrowing() {
        return hardCopyBorrowing;
    }

    public void setHardCopyBorrowing(HardCopyBorrowing hardCopyBorrowing) {
        this.hardCopyBorrowing = hardCopyBorrowing;
    }

    public FineDto toDto() { return new FineDto(id, amount, state, hardCopyBorrowing.getId()); }
}
