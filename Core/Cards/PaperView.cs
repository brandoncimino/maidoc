namespace maidoc.Core.Cards;

/// <summary>
/// Provides information about all the stuff on the table, without the ability to manipulate any of it.
/// Essentially the "read-only" version of <see cref="PaperPusher"/>
/// </summary>
public interface IPaperView {
    public IPaperZone GetZoneOfCard(SerialNumber serialNumber);

    public ICellOccupant? GetCellOccupant(CellAddress cellAddress);
}