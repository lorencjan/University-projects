package com.fit.vut.Library.dtos;

import com.fit.vut.Library.entities.Fine;
import com.fit.vut.Library.entities.HardCopyBorrowing;
import com.fit.vut.Library.enums.FineEnum;

public class FineDto extends DtoBase {

    private int amount;

    private FineEnum state;

    private Long borrowingId;

    public FineDto(){}

    public FineDto(Long id, int amount, FineEnum state, Long borrowingId){
        super(id);
        this.amount = amount;
        this.state = state;
        this.borrowingId = borrowingId;
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

    public Long getBorrowingId() {
        return borrowingId;
    }

    public void setBorrowingId(Long id) {
        this.borrowingId = id;
    }

    public Fine toEntity() {return new Fine(id, amount, state, new HardCopyBorrowing(borrowingId));}
}
