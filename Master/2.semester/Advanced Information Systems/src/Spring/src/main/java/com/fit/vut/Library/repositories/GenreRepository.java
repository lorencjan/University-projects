package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.Genre;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;
import java.util.List;

@Repository
public interface GenreRepository extends JpaRepository<Genre, Long> {

    @Query(value = "SELECT g from Genre g WHERE g.name = :name")
    List<Genre> findByName(@Param("name") String name);
}
