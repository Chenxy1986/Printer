using NanoImprinter.Procedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.Model
{
    public class WorkProcedureManager
    {
        /// <summary>
        /// 考虑Load/Unload增加工序
        /// </summary>
        private readonly Dictionary<string, WorkProcedure> _procedures = new Dictionary<string, WorkProcedure>();
        private List<WorkProcedure> _autoProcedures;
        private readonly NanoImprinterModel _model;
        private static WorkProcedureManager _instance;


        public static WorkProcedureManager Instance => _instance ?? new WorkProcedureManager();

        public IReadOnlyCollection<string> ProcedureNames => _procedures.Keys;


        public WorkProcedureManager()
        {
            LoadProcesure();
            _autoProcedures = new List<WorkProcedure>();
            foreach (var keyPair in _procedures)
            {
                if (Enum.TryParse(keyPair.Key, out ProcedureName name))
                    _autoProcedures.Add(keyPair.Value);
            }
        }



        private void LoadProcesure()
        {
            AddProcedures(new PickProcedure());
            AddProcedures(new FindRotateCenterProcedure());
            //AddProcedures(new PositionProcedure());
            AddProcedures(new GlueProcedure());
            AddProcedures(new ImprinterProcedure());
            //AddProcedures(new UVFixedProcedure());
            AddProcedures(new ReleaseProcedure());
            AddProcedures(new PutProcesure());
        }

        private void AddProcedures(WorkProcedure procedure)
        {
            _procedures.Add(procedure.Name, procedure);
        }


        public WorkProcedure GetWorkProcedure(ProcedureName name)
        {
            if (_procedures.TryGetValue(name.ToString(), out WorkProcedure workProcedure))
            {
                return workProcedure;
            }
            return null;
        }

        public bool ProcessPutProcedure()
        {
            var putProcedure = GetWorkProcedure(ProcedureName.PutProcedure);
            return Process(putProcedure);
        }

        public bool ProcessPickProcedure()
        {
            var putProcedure = GetWorkProcedure(ProcedureName.PickProcedure);
            return Process(putProcedure);
        }

        public bool ProcessAutoProcedure()
        {
            try
            {
                foreach (var op in _autoProcedures)
                {
                    if (_model.Status > MachineStatus.Running)
                        break;
                    Process(op);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool Process(WorkProcedure procedure)
        {
            try
            {
                if (procedure != null)
                {
                    procedure.Excute();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }

    public enum ProcedureName
    {
        PickProcedure,
        FindRotateCenterProcedure,
        PositionProcedure,
        GlueProcedure,
        ImprinterProcedure,
        UVFixedProcedure,
        ReleaseProcedure,
        PutProcedure,
    }
}
