package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.*;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ReservationRepository extends JpaRepository <Reservation, Long> {

    @Query(value = "SELECT r from Reservation r WHERE r.hardCopyExemplar = :copy")
    List<Reservation> findByHardCopyExemplar(@Param("copy")HardCopyExemplar copy);
}
