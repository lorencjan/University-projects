package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.*;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface FineRepository extends JpaRepository <Fine, Long> {

    @Query("SELECT f FROM Fine f WHERE f.state = :state")
    List<Fine> findByState (@Param("state") int state);
}

